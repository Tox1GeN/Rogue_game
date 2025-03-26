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
        public Room (int rows = 20, int cols = 40, bool skipGeneration = false)
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

            if (!skipGeneration)
            {
                var builder = new Generation.DefaultDungeonBuilder();
                var director = new Generation.DungeonDirector(builder);
                //Room firstLevel = director.ConstructFirstLevelDungeon(Rows, Columns);
                //Room secondLevel = director.ConstructSecondLevelDungeon(Rows, Columns);
                Room thirdLevel = director.ConstructThirdLevelDungeon(Rows, Columns);

                Grid = thirdLevel.Grid;
                PlayerPosition = thirdLevel.PlayerPosition;
            }
        }

        //// Initialize the room
        //public BuildingRoom()
        //{
        //    // each cell in the grid
        //    for (int i = 0; i < 20; i++)
        //    {
        //        for (int j = 0; j < 40; j++)
        //        {
        //            Grid[i, j] = new Cell();
        //        }
        //    }

        //    // Generate the Level:

        //    // Filling the walls:
        //    for (int i = 3; i < 40; i++)
        //    {
        //        Grid[0, i].IsWall = true;
        //    }
        //    for (int i = 0; i < 40; i++)
        //    {
        //        Grid[19, i].IsWall = true;
        //    }
        //    for (int i = 0; i < 20; i++)
        //    {
        //        Grid[i, 39].IsWall = true;
        //    }
        //    for (int i = 3; i < 20; i++)
        //    {
        //        Grid[i, 0].IsWall = true;
        //    }

        //    // Put items on the floor:
        //    // Sword in the cell [2,10]
        //    Sword sword = new Sword("Excalibur", 10);

        //    Grid[2, 10].Items.Push(new LegendaryEffect(sword));

        //    // Some gold in the cell [1,1]
        //    Grid[1, 1].Items.Push(new Mace("Morning Star", 5));

        //    // Spawn the Player
        //    Grid[0, 0].IsPlayerHere = true;
        //    PlayerPosition = (0, 0);
        //}

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