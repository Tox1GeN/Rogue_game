using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class DefaultDungeonBuilder : IDungeonBuilder
    {
        private Room? _room;
        private Room BuildingRoom => _room ?? throw new InvalidOperationException("Room is not initialized.");

        private Random _rng = new Random();
        public void InitGrid(int rows, int cols)
        {
            _room = new Room(rows, cols, skipGeneration: true);
        }

        public void EmptyDungeon()
        {
            for (int i = 0; i < BuildingRoom.Rows; i++)
            {
                for ( int j = 0; j < BuildingRoom.Columns; j++)
                {
                    BuildingRoom.Grid[i, j].IsWall = false;
                }
            }
        }

        public void FilledDungeon()
        {
            for (int i = 0; i < BuildingRoom.Rows; i++)
            {
                for (int j = 0; j < BuildingRoom.Columns; j++)
                {
                    BuildingRoom.Grid[i, j].IsWall = true;
                }
            }
        }

        // --- Additional Strategies ---
        public void AddPaths()
        {
            // Assume the dungeon is filled. Carve a path using a simple random walk.
            int currentRow = _rng.Next(1, BuildingRoom.Rows - 1);
            int currentCol = _rng.Next(1, BuildingRoom.Columns - 1);
            int steps = (BuildingRoom.Rows * BuildingRoom.Columns) / 4;  // arbitrary step count

            for (int step = 0; step < steps; step++)
            {
                BuildingRoom.Grid[currentRow, currentCol].IsWall = false;
                // Randomly decide a direction: up, down, left, right
                int dir = _rng.Next(4);
                switch (dir)
                {
                    case 0: if (currentRow > 1) currentRow--; break; // up
                    case 1: if (currentRow < BuildingRoom.Rows - 2) currentRow++; break; // down
                    case 2: if (currentCol > 1) currentCol--; break; // left
                    case 3: if (currentCol < BuildingRoom.Columns - 2) currentCol++; break; // right
                }
            }
        }

        public void AddChambers()
        {
            // Carve out several random rectangular chambers (like holes in a cheese)
            int chamberCount = 3;  // arbitrary number of chambers
            for (int c = 0; c < chamberCount; c++)
            {
                // Random top-left position ensuring the chamber fits
                int chamberWidth = _rng.Next(3, 8);
                int chamberHeight = _rng.Next(3, 6);
                int row = _rng.Next(1, BuildingRoom.Rows - chamberHeight - 1);
                int col = _rng.Next(1, BuildingRoom.Columns - chamberWidth - 1);

                for (int i = row; i < row + chamberHeight; i++)
                {
                    for (int j = col; j < col + chamberWidth; j++)
                    {
                        BuildingRoom.Grid[i, j].IsWall = false;
                    }
                }
            }
        }

        public void AddCentralRoom()
        {
            // Carve a large room in the center
            int roomHeight = BuildingRoom.Rows / 2;
            int roomWidth = BuildingRoom.Columns / 2;
            int startRow = (BuildingRoom.Rows - roomHeight) / 2;
            int startCol = (BuildingRoom.Columns - roomWidth) / 2;

            for (int i = startRow; i < startRow + roomHeight; i++)
            {
                for (int j = startCol; j < startCol + roomWidth; j++)
                {
                    BuildingRoom.Grid[i, j].IsWall = false;
                }
            }
        }

        public void AddItems()
        {

            var note = new Rogue.Models.UnusableItems.MysteriousNote("Strange Note", "A old piece of paper that I found after I've woke up", "This is a note. It says: 'You are the chosen one.'");

            BuildingRoom.Grid[14, 17].Items.Push(note);
        }

        public void AddWeapons()
        {

            var sword = new Rogue.Models.Weapons.Sword("Excalibur", damage: 10);

            BuildingRoom.Grid[12, 13].Items.Push(new Rogue.Decorators.LegendaryEffect(sword));
        }


        public void PlacePlayer(int x, int y)
        {
            BuildingRoom.Grid[y, x].IsWall = false;
            BuildingRoom.PlayerPosition = (y, x);
            BuildingRoom.Grid[y, x].IsPlayerHere = true;
        }

        public Room GetResult() => BuildingRoom;
    }
}
