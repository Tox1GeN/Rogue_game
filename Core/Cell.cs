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
        public char GetDisplayCell()
        {
            if (IsPlayerHere)
                return '¶';
            if (IsWall)
                return '█';
            if (Items.Count > 0)
                return '*';
            if (Enemy != null)
                return Enemy.ToString()[0];
            // Otherwise it is an empty cell
            return ' ';
        }
    }
}
