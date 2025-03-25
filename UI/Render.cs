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
        private const int actionWindowY = 20;  // e.g., below a 20-row game grid
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
            for (int y = 0; y < actionWindowHeight; y++)
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
            int sidePanelX = 80;
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
    }
}
