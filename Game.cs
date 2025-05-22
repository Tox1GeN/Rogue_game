using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Core.Generation.Builders;
using Rogue.Core.Generation.Interfaces;
using Rogue.Models;
using Rogue.Network.Client;
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
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                _inputHandlerChain.Handle(keyInfo.Key, _player, _currentRoom);
                _player.UpdateEffectsPerTurn();
                Render.Instance.RenderSidePanel(_player, _currentRoom);
                Render.Instance.RenderMonsterPanel(_player, _currentRoom);
                Render.Instance.RenderInstructions(_instructions);

                if (ExitHandler.GameShouldExit)
                {
                    running = false;
                }
            }

            Environment.Exit(0);
        }
    }
}