using Rogue.Models;
using Rogue.Models.Weapons;
using Rogue.Models.Weapons.TwoHanded;
using Rogue.Decorators;
using Rogue.Models.Currency;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core
{
    public class Room
    {
        // Game field
        public Cell[,] Grid { get; private set; } // = new Cell[20, 40];
        public int Rows { get; }
        public int Columns { get; }

        // Where is the Player?
        public (int row_Y, int col_X) PlayerPosition { get; set; }
        public Room (int rows = 20, int cols = 40, bool skipGeneration = true)
        {
            Rows = rows;
            Columns = cols;
            Grid = new Cell[Rows, Columns];
            for ( int i = 0; i < Rows; i++)
            {
                for ( int j = 0; j < Columns; j++)
                {
                    Grid[i, j] = new Cell();
                }
            }
        }

        // Render the room
        public void Render()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var (symbol, color) = Grid[i, j].GetDisplayCell();
                    Console.ForegroundColor = color;
                    Console.Write(symbol);
                    Console.ResetColor();
                    // Reset so subsequent cells don’t inherit the color
                }
                Console.WriteLine();
            }
        }

        public Item? RemoveTopItemAt(int row_Y, int col_X)
        {
            if (row_Y < 0 || row_Y > 19 || col_X < 0 || col_X > 39 || Grid[row_Y, col_X].IsWall)
                return null;
            return Grid[row_Y, col_X].Items.Count > 0 ? Grid[row_Y, col_X].Items.Pop() : null;
        }
        public void ReceiveDropItem(int row_Y, int col_X, Item dropItem)
        {
            if (row_Y < 0 || row_Y > 19 || col_X < 0 || col_X > 39 || Grid[row_Y, col_X].IsWall)
                return;
            Grid[row_Y, col_X].Items.Push(dropItem);
        }
    }
}