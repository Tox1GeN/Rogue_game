using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Core.Generation.Builders;
using Rogue.Core.Generation.Interfaces;
using Rogue.Models;
using Rogue.Network.Client;
using Rogue.Network.Dto;
using Rogue.Network.Server;
using Rogue.UI;
using Rogue.UI.Input_Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rogue
{
    public class Game
    {
        private Room _currentRoom;
        private Player _player;
        private InputHandler _inputHandlerChain;
        private List<string> _instructions;
        private BuildResult _buildResult;

        public Game()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.SetWindowSize(120, 40);
            Console.SetBufferSize(120, 40);
            Console.CursorVisible = false;

            _player = new Player();

            IBuilder dungeonBuilder = new DungeonGeneration();
            IBuilder instructionBuilder = new InstructionBuilder();
            IBuilder chainBuilder = new ChainBuilder();

            CompositeBuilder compositeBuilder = new CompositeBuilder();
            compositeBuilder.AddBuilder(dungeonBuilder)
                            .AddBuilder(instructionBuilder)
                            .AddBuilder(chainBuilder);

            DungeonDirector director = new DungeonDirector(compositeBuilder);
            _buildResult = director.ConstructDungeon(20, 40, _player, 3, 3);

            _currentRoom = _buildResult.Dungeon!;
            _instructions = _buildResult.Instructions;
            List<InputHandler> inputHandlers = _buildResult.InputHandlers;
            _inputHandlerChain = inputHandlers.Count > 0 ? inputHandlers[0] : new DefaultHandler();

            _currentRoom.Render();
            Render.Instance.RenderSidePanel(_player, _currentRoom);
            Render.Instance.RenderMonsterPanel(_player, _currentRoom);
            Render.Instance.RenderInstructions(_instructions);
        }

        public void Run()
        {
            bool running = true;
            while (running)
            {
                foreach (var enemy in _currentRoom.Grid
                                         .Cast<Cell>()
                                         .Where(cell => cell.Enemy != null)
                                         .Select(cell => cell.Enemy!))
                {
                    enemy.HasActedThisRound = false;
                }

                // Read player input and apply it
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                _inputHandlerChain.Handle(keyInfo.Key, _player, _currentRoom);
                _player.UpdateEffectsPerTurn();

                // ENEMY PHASE: let each enemy in the room take its turn
                // We need a list of all enemies currently on the grid.
                var enemiesToAct = new List<Enemy>();
                for (int r = 0; r < _currentRoom.Rows; r++)
                {
                    for (int c = 0; c < _currentRoom.Columns; c++)
                    {
                        var cell = _currentRoom.Grid[r, c];
                        if (cell.Enemy != null && cell.Enemy.Health > 0)
                        {
                            // Set its Position so behavior logic is accurate
                            cell.Enemy.Position = (r, c);
                            enemiesToAct.Add(cell.Enemy);
                        }
                    }
                }

                // For each enemy, call TakeTurn. Build up a local GameUpdateDTO to track cell changes.
                var localUpdate = new GameUpdateDTO();
                foreach (var enemy in enemiesToAct)
                {
                    // Skip if already acted this round (if you use that flag), or dead
                    if (enemy.HasActedThisRound || enemy.Health <= 0)
                        continue;

                    enemy.TakeTurn(_currentRoom, new[] { _player }, localUpdate);
                    enemy.HasActedThisRound = true;
                }
                // After all have acted, reset HasActedThisRound so next loop they can act again
                foreach (var enemy in enemiesToAct)
                    enemy.HasActedThisRound = false;

                // 3) Re‐render everything, including any enemy moves/attacks
                Render.Instance.RenderSidePanel(_player, _currentRoom);
                Render.Instance.RenderMonsterPanel(_player, _currentRoom);
                Render.Instance.RenderInstructions(_instructions);

                // (Optional) If you want to visually show any changes to floor cells,
                // you can iterate localUpdate.ChangedCells and redraw them here:
                foreach (var cu in localUpdate.ChangedCells)
                    Render.Instance.RedrawCell(cu.Row, cu.Col, _currentRoom);


                if (ExitHandler.GameShouldExit)
                {
                    running = false;
                }
            }

            Environment.Exit(0);
        }
    }
}