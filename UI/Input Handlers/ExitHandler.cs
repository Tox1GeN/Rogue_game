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
                Render.Instance.StartNewActionMessage();
                Render.Instance.AddActionLine("Exiting the game...");
                Render.Instance.FinalizeActionMessage();
                return;
            }

            base.Handle(key, player, currentRoom);
        }
    }
}
