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
            // Start at a random interior position
            int currentRow = _rng.Next(1, BuildingRoom.Rows - 1);
            int currentCol = _rng.Next(1, BuildingRoom.Columns - 1);
            int totalSteps = (BuildingRoom.Rows * BuildingRoom.Columns);  // total carving steps (adjustable)
            int[] dRow = { -1, 1, 0, 0 };  // direction vectors (up, down, left, right)
            int[] dCol = { 0, 0, -1, 1 };

            // Choose an initial random direction
            int currentDir = _rng.Next(4);

            for (int step = 0; step < totalSteps; step++)
            {
                BuildingRoom.Grid[currentRow, currentCol].IsWall = false;  // carve out current cell

                // Decide whether to turn or continue straight
                if (_rng.NextDouble() < 0.3)
                {
                    // 30% chance to turn (pick a new direction not directly back on itself)
                    int newDir;
                    do
                    {
                        newDir = _rng.Next(4);
                    } while ((currentDir == 0 && newDir == 1) ||
                             (currentDir == 1 && newDir == 0) ||
                             (currentDir == 2 && newDir == 3) ||
                             (currentDir == 3 && newDir == 2));
                    currentDir = newDir;
                }

                // Compute next cell in the chosen direction
                int nextRow = currentRow + dRow[currentDir];
                int nextCol = currentCol + dCol[currentDir];

                // If next cell is out of bounds or would hit the border, choose a different direction
                if (nextRow < 1 || nextRow >= BuildingRoom.Rows - 1 ||
                    nextCol < 1 || nextCol >= BuildingRoom.Columns - 1)
                {
                    continue; // skip this step (or optionally pick a different direction)
                }
                // If next cell is already carved open and we risk creating a wide open area, 
                // randomly decide to turn to avoid widening the corridor
                if (!BuildingRoom.Grid[nextRow, nextCol].IsWall && _rng.NextDouble() < 0.8)
                {
                    // 80% chance to turn away from carving an already open cell (preserve narrow tunnel)
                    currentDir = _rng.Next(4);
                    continue;
                }

                // Move into the next cell
                currentRow = nextRow;
                currentCol = nextCol;
            }
        }

        public void AddChambers()
        {
            // Carve out several random rectangular chambers (like holes in a cheese)
            int chamberCount = 6;  // arbitrary number of chambers
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

        private void PlaceObjectsRandomly<T>(int count, Func<T> createFunc, Action<Cell, T> placeAction) where T : Models.Item
        {
            int placed = 0;
            // Avoid infinite loops by limiting attempts.
            int attemps = 0;
            while (placed < count && attemps < 1000)
            {
                int r = _rng.Next(0, BuildingRoom.Rows);
                int c = _rng.Next(0, BuildingRoom.Columns);
                if (!BuildingRoom.Grid[r, c].IsWall && BuildingRoom.Grid[r, c].Enemy == null)
                {
                    placeAction(BuildingRoom.Grid[r, c], createFunc());
                    placed++;
                }
                attemps++;
            }
        }

        public void AddItems()
        {
            PlaceObjectsRandomly(1,
                () => new Rogue.Models.UnusableItems.MysteriousNote(
                    "Strange note",
                    "It looks like a joke",
                    "You are the chosen one"),
                (cell, item) => cell.Items.Push(item));
            PlaceObjectsRandomly(2,
                () => new Rogue.Models.UsableItems.Potion("Little Health Potion", 10),
                (cell, item) => cell.Items.Push(item));
        }

        public void AddWeapons()
        {
            PlaceObjectsRandomly(1,
                () => new Rogue.Models.Weapons.Sword("Wooden Sword", 2),
                (cell, item) => cell.Items.Push(item));
        }

        public void AddEnemies()
        {
            // Let's say we want to spawn 3 enemies.
            int enemyCount = 3;
            int attempts = 0;
            // Avoid infinite loops by limiting attempts.
            while (enemyCount > 0 && attempts < 1000)
            {
                int r = _rng.Next(0, BuildingRoom.Rows);
                int c = _rng.Next(0, BuildingRoom.Columns);
                var cell = BuildingRoom.Grid[r, c];
                // Check if cell is truly empty.
                if (!cell.IsWall && cell.Items.Count == 0 && !cell.IsPlayerHere && cell.Enemy == null)
                {
                    cell.Enemy = new Rogue.Models.Enemy("Skeleton", health: 6, attackPower: 3);
                    enemyCount--;
                }
                attempts++;
            }
        }


        public void AddModifiedWeapons()
        {
            // Example: add 2 weapons with a modifier.
            PlaceObjectsRandomly(1,
                () =>
                {
                    // Wrap a sword with a powerful effect as an example.
                    var sword = new Rogue.Models.Weapons.Sword("Saber", 3);
                    return new Rogue.Decorators.PowerfulEffect(sword);
                },
                (cell, item) => cell.Items.Push(item));
        }

        public void PlacePlayer(int row, int col)
        {
            BuildingRoom.Grid[row, col].IsWall = false;
            BuildingRoom.PlayerPosition = (row, col);
            BuildingRoom.Grid[row, col].IsPlayerHere = true;
        }

        public Room GetResult() => BuildingRoom;
    }
}
