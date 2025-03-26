using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI
{
    public sealed class Render
    {

        // The single instance of Render (lazy-initialized)
        private static Render? _instance;
        public static Render Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Render();
                }
                return _instance;
            }
        }

        private Render()
        {
            _currentActionLines = new List<string>();

            // Any of one-time initialization code goes here
        }

        // Store the lines for the *current* action.
        private List<string> _currentActionLines;

        // Configuration for the action window.
        private const int actionWindowX = 0;
        private const int actionWindowY = 21;  // e.g., below a 20-row game grid
        private const int actionWindowWidth = 80;
        private const int actionWindowHeight = 10;

        /// Call this before starting a new action that may produce multiple lines of output.
        /// Clears the buffer (but not the console).
        public void StartNewActionMessage()
        {
            _currentActionLines.Clear();
        }

        /// Instead of Console.WriteLine, call this to add lines to the current action's messages.
        /// We split on newline in case the string has multiple lines.
        public void AddActionLine(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // If the string has multiple lines (due to \n), split them up.
            string[] lines = message.Split('\n');
            foreach (var line in lines)
            {
                _currentActionLines.Add(line.TrimEnd('\r'));
            }
        }

        public void FinalizeActionMessage()
        {
            ClearActionWindow();

            // Now, print each line in the region, up to actionWindowHeight lines.
            int lineIndex = 0;
            foreach (var line in _currentActionLines)
            {
                if (lineIndex >= actionWindowHeight)
                    break; // No more space in the window

                // If a line is longer than actionWindowWidth, either wrap or truncate.
                // Here we truncate for simplicity.
                string truncated = line.Length > actionWindowWidth
                                   ? line.Substring(0, actionWindowWidth)
                                   : line;

                Console.SetCursorPosition(actionWindowX, actionWindowY + lineIndex);
                Console.Write(truncated);
                lineIndex++;
            }
        }

        // Clears the rectangular area where action messages go.
        private void ClearActionWindow()
        {
            for (int y = 0; y < actionWindowHeight - 1; y++)
            {
                Console.SetCursorPosition(actionWindowX, actionWindowY + y);
                Console.Write(new string(' ', actionWindowWidth));
            }
        }



        public void RedrawCell(int row, int col, Room currentRoom)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(currentRoom.Grid[row, col].GetDisplayCell());
        }

        public void RenderSidePanel(Player player, Room currentRoom)
        {
            int sidePanelX = 45;
            //int sidePanelY = 0;

            for(int i = 0; i < 25; i++)
            {
                Console.SetCursorPosition(sidePanelX, i);
                Console.Write(new string(' ', 30));
            }


            // TODO: test use write instead of writeline
            // Display Player Stats
            Console.SetCursorPosition(sidePanelX, 0);
            Console.WriteLine("Player Stats:");
            Console.SetCursorPosition(sidePanelX, 1);
            Console.WriteLine($"Str: {player.Strength}  Dex: {player.Dexterity}");
            Console.SetCursorPosition(sidePanelX, 2);
            Console.WriteLine($"Health: {player.Health}  Luck: {player.Luck}");
            Console.SetCursorPosition(sidePanelX, 3);
            Console.WriteLine($"Agg: {player.Aggression}  Wis: {player.Wisdom}");


            int coins = player.Coins.Count, gold = player.GoldValue.Count;
            //foreach (var item in player.Inventory.Items)
            //{
            //    switch(item.GetDisplayName())
            //    {
            //        case "Coin":
            //            coins++;
            //            break;
            //        case "Gold":
            //            gold++;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            Console.SetCursorPosition(sidePanelX, 5);
            Console.WriteLine($"Coins: {coins}  Gold: {gold}");

            // Equipped items
            Console.SetCursorPosition(sidePanelX, 7);
            Console.WriteLine("Equipped Items:");
            Console.SetCursorPosition(sidePanelX, 8);
            var leftHand = player.Hands[0] != null ? player.Hands[0]!.GetDisplayName() : "(empty)";
            var rightHand = player.Hands[1] != null ? player.Hands[1]!.GetDisplayName() : "(empty)";
            Console.WriteLine($" Left Hand: {leftHand}");
            Console.SetCursorPosition(sidePanelX, 9);
            Console.WriteLine($" Right Hand: {rightHand}");


            // TODO: test `yPos`
            // Inventory
            Console.SetCursorPosition(sidePanelX, 11);
            Console.WriteLine("Inventory:");
            int yPos = 12;
            for (int i = 0; i < player.Inventory.Items.Count; i++)
            {
                Console.SetCursorPosition(sidePanelX, yPos++);
                Console.WriteLine($"{i}. {player.Inventory.Items[i].GetDisplayName()}");
            }

            // TODO: Show all the items in the cell, not only the top
            // Item on the ground 
            var (pRow, pCol) = currentRoom.PlayerPosition;
            if (currentRoom.Grid[pRow, pCol].Items.Count > 0)
            {
                Console.SetCursorPosition(sidePanelX, ++yPos);
                Console.WriteLine("Item on ground:");
                var topItem = currentRoom.Grid[pRow, pCol].Items.Peek();
                Console.SetCursorPosition(sidePanelX, ++yPos);
                Console.WriteLine($" {topItem.GetDisplayName()}");
            }
        }

        public void RenderInstructions(IEnumerable<string> lines)
        {
            int instrX = 80;        // Where to start printing instructions
            int instrY = 10;        // Which row to start printing
            int instrPanelWidth = 40;  // Width of the instruction panel
            int instrHeight = 5;    // Number of lines reserved for instructions

            // 1. Clear the instruction area
            for (int i = 0; i < instrHeight; i++)
            {
                Console.SetCursorPosition(instrX, instrY + i);
                // Write enough spaces to clear up to instrPanelWidth
                Console.Write(new string(' ', instrPanelWidth));
            }

            // 2. Print each instruction line, padding/truncating as needed
            int y = instrY;
            foreach (string line in lines)
            {
                if (y >= instrY + instrHeight) break;  // Don't overflow the area
                Console.SetCursorPosition(instrX, y++);

                // Truncate if it's too long, otherwise pad with spaces
                string adjustedLine = line.Length > instrPanelWidth
                    ? line.Substring(0, instrPanelWidth)
                    : line.PadRight(instrPanelWidth);

                Console.Write(adjustedLine);
            }
        }


        public void RenderMonsterPanel(Player player, Room currentRoom)
        {
            int monsterPanelX = 80;    // starting column for monster panel (adjusted for layout)
            int panelWidth = 25;       // width of the monster panel
            int panelHeight = 25;      // number of lines to clear (covering rows 0-24)

            // 1. Clear the monster panel area
            for (int y = 0; y < panelHeight; y++)
            {
                Console.SetCursorPosition(monsterPanelX, y);
                Console.Write(new string(' ', panelWidth));
            }

            // 2. Gather nearby monsters within 5x5 area of player
            var (pRow, pCol) = currentRoom.PlayerPosition;
            int minRow = Math.Max(0, pRow - 2);
            int maxRow = Math.Min(currentRoom.Rows - 1, pRow + 2);
            int minCol = Math.Max(0, pCol - 2);
            int maxCol = Math.Min(currentRoom.Columns - 1, pCol + 2);

            List<Enemy> nearbyEnemies = new List<Enemy>();
            for (int r = minRow; r <= maxRow; r++)
            {
                for (int c = minCol; c <= maxCol; c++)
                {
                    var enemy = currentRoom.Grid[r, c].Enemy;
                    if (enemy != null)
                    {
                        nearbyEnemies.Add(enemy);
                    }
                }
            }

            if (nearbyEnemies.Count == 0)
            {
                // No enemies in range; we could optionally show "No monsters nearby" or leave blank
                return;
            }

            // 3. Print header and each enemy's info
            Console.SetCursorPosition(monsterPanelX, 0);
            Console.Write("Nearby Monsters:");
            int line = 1;
            foreach (var enemy in nearbyEnemies)
            {
                if (line >= panelHeight) break;  // safety check to avoid overflow
                Console.SetCursorPosition(monsterPanelX, line++);
                Console.Write($"- {enemy.Name}  HP:{enemy.Health}  DMG:{enemy.AttackPower}");
            }
        }
    }
}
