using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core
{
    internal class Room
    {
        // Game field
        public Cell[,] Grid { get; } = new Cell[20, 40];

        // Player's position
        public (int row_Y, int col_X) PlayerPosition { get; set; }

        // Initialize the room
        public Room()
        {
            // each cell in the grid
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    Grid[i, j] = new Cell();
                }
            }

            // Spawn the Player
            Grid[0, 0].IsPlayerHere = true;
            PlayerPosition = (0, 0);
        }

        // Render the room 
        // TODO: change the render logic line by line, not char by char to make it faster and smother
        public void Render()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    Console.Write(Grid[i, j].GetDisplayCell());
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