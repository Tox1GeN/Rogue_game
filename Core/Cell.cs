using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;

namespace Rogue.Core
{
    public class Cell
    {
        // Each cell can be a wall or empty cell
        public bool IsWall { get; set; } = false;

        // Empty cell can have one item or even more in it
        // List of items in the cell
        public Stack<Item> Items { get; set; } = new Stack<Item>();

        // TODO: comment out this field and replace it with Player PlayerOccupant
        public bool IsPlayerHere { get; set; } = false;

        public Player? PlayerOccupant { get; set; } = null; // which Player is in the cell (if any)

        // Enemy in the cell
        public Enemy? Enemy { get; set; }



        // Get character of the current cell for a rendering
        public (char symbol, ConsoleColor color) GetDisplayCell()
        {
            if (PlayerOccupant != null)
            {
                // Represent players with the same glyph but different colors
                return ('¶', PlayerOccupant.Color);  // use the player's assigned color
            }

            if (IsWall)
                return ('█', ConsoleColor.DarkGray);

            // Make enemy render always on top of any item
            if (Items.Count > 0 && Enemy == null)
            {
                Item topItem = Items.Peek();
                char itemChar = topItem.GetDisplayName()[0];
                if (topItem.CanEquip)    return (itemChar, ConsoleColor.Cyan);    // e.g., weapons/armor
                else if (topItem.CanUse) return (itemChar, ConsoleColor.Magenta); // e.g., potions
                else                     return ('I', ConsoleColor.Yellow);       // other items
            }
            if (Enemy != null)
                // First letter of the enemy’s name, colored red
                return (Enemy.Name[0], ConsoleColor.Red);

            // Otherwise an empty floor
            return (' ', ConsoleColor.Black);
        }
    }
}
