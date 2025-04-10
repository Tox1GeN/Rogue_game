using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI.Input_Handlers
{
    public class MovementHandler : InputHandler
    {
        public override void Handle(ConsoleKey key, Player player, Room currentRoom)
        {
            switch(key)
            {
                case Controls.UpKey:
                    player.Move(-1, 0, currentRoom);  // move up
                    return;  // handled, stop chain
                case Controls.DownKey:
                    player.Move(1, 0, currentRoom);   // move down
                    return;
                case Controls.LeftKey:
                    player.Move(0, -1, currentRoom);  // move left
                    return;
                case Controls.RightKey:
                    player.Move(0, 1, currentRoom);   // move right
                    return;
            }

            // If not one of the movement keys, pass it to the next handler in the chain.
            base.Handle(key, player, currentRoom);
        }
    }
}
