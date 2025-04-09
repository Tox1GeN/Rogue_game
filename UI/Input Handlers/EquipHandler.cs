using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI.Input_Handlers
{
    public class EquipHandler : InputHandler
    {
        public override void Handle(ConsoleKey key, Player player, Room currentRoom)
        {
            if (key == ConsoleKey.F)
            {
                PlayerActions.PressEquip(player, currentRoom);
                return;
            }

            // pass to the next
            base.Handle(key, player, currentRoom);
        }
    }
}
