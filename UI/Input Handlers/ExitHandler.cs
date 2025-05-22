using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI.Input_Handlers
{
    public class ExitHandler : InputHandler
    {
        public static bool GameShouldExit { get; private set; } = false;

        public override void Handle(ConsoleKey key, Player player, Room currentRoom)
        {
            if (key == ConsoleKey.Escape)
            {
                GameShouldExit = true;
                MessageBuffer.Begin();
                MessageBuffer.Add("Exiting the game...");
                MessageBuffer.Commit();
                return;
            }

            base.Handle(key, player, currentRoom);
        }
    }
}
