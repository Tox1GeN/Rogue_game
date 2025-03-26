using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core
{
    public class Cell
    {
        // Each cell can be a wall or empty cell
        public bool IsWall { get; set; } = false;

        // Empty cell can have one item or even more in it
        // List of items in the cell
        public Stack<Rogue.Models.Item> Items { get; set; } = new Stack<Rogue.Models.Item>();

        public bool IsPlayerHere { get; set; } = false;

        // Enemy in the cell
        public Rogue.Models.Enemy? Enemy { get; set; }

        // Get character of the current cell for a rendering
        public (char symbol, ConsoleColor color) GetDisplayCell()
        {
            if (IsPlayerHere)
                return ('¶', ConsoleColor.White);

            if (IsWall)
                return ('█', ConsoleColor.DarkGray);

            if (Items.Count > 0)
                if (Items.Peek().CanEquip)
                    return (Items.Peek().GetDisplayName()[0], ConsoleColor.Cyan);
                else
                    return ('I', ConsoleColor.Yellow);
            if (Enemy != null)
                // First letter of the enemy’s name, colored red
                return (Enemy.Name[0], ConsoleColor.Red);

            // Otherwise an empty floor
            return (' ', ConsoleColor.Black);
        }
    }
}
