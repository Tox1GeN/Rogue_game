using Rogue.Models;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class InstructionBuilder
    {
        private List<string> _instructions = new List<string>();

        public InstructionBuilder BuildInstructions(Room room, Player player)
        {
            _instructions.Clear();

            // Movement controls
            _instructions.Add("Use W/A/S/D to to move.");

            // If any cell has items, add instruction for pickup.
            bool hasItemsOnGround = false;
            for (int i = 0; i < room.Rows && !hasItemsOnGround; i++)
            {
                for (int j = 0; j < room.Columns; j++)
                {
                    if (room.Grid[i, j].Items.Count > 0)
                    {
                        hasItemsOnGround = true;
                        break;
                    }
                }
            }

            // Pickup control
            if (hasItemsOnGround)
                _instructions.Add("Press E to pick up items.");

            // Droping control
            if (player.Inventory.Items.Count > 0)
            {
                _instructions.Add("Press G to drop an item.");
            }

            // Equiping control
            bool anyEquipable = player.Inventory.Items.Any(item => item.CanEquip);
            if(anyEquipable)
            {
                _instructions.Add("Press F to equip an item from invetory.");
            }

            // 5. Unequipping items (if player has something in hands)
            if (player.Hands[0] != null || player.Hands[1] != null)
            {
                _instructions.Add("Press U to unequip currently held item.");
            }


            // Stage 3 ???
            // combat ???

            return this;
        }

        public void Display()
        {
            Render.Instance.RenderInstructions(_instructions);
        }
    }
}
