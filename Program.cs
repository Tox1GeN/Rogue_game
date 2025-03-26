using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Models;
using Rogue.Models.Weapons;
using Rogue.UI;
using System.Numerics;

namespace Rogue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 40);
            Console.SetBufferSize(120, 40);


            // Turn of blinking cursor for a cleaner UI
            Console.CursorVisible = false;

            // Launch the game
            Room currentRoom = new Room();
            Player player = new Player();

            currentRoom.Render();

            Render.Instance.RenderSidePanel(player, currentRoom);
            Render.Instance.RenderMonsterPanel(player, currentRoom);

            var instrBuilder = new InstructionBuilder();
            instrBuilder.BuildInstructions(currentRoom, player).Display();

            while (true)
            {
                // Read user input without displaying it.
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Store player's current position before moving.
                var (oldRow, oldCol) = currentRoom.PlayerPosition;               
                

                // Process movement keys (W, A, S, D) and other actions.
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        player.Move(-1, 0, currentRoom);
                        break;
                    case ConsoleKey.S:
                        player.Move(1, 0, currentRoom);
                        break;
                    case ConsoleKey.A:
                        player.Move(0, -1, currentRoom);
                        break;
                    case ConsoleKey.D:
                        player.Move(0, 1, currentRoom);
                        break;
                    case ConsoleKey.E:
                        PlayerActions.PressPickUp(player, currentRoom);
                        break;
                    case ConsoleKey.G:
                        PlayerActions.PressDrop(player, currentRoom);
                        break;
                    case ConsoleKey.F:
                        PlayerActions.PressEquip(player, currentRoom);
                        break;
                    case ConsoleKey.U:
                        PlayerActions.PressUnequip(player, currentRoom);
                        break;
                    default:
                        break;
                }
                instrBuilder.BuildInstructions(currentRoom, player).Display();
            }
        }
    }
}
