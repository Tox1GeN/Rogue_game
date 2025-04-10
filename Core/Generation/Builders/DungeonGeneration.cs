using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core.Generation.Interfaces;

namespace Rogue.Core.Generation.Builders
{
    public class DungeonGeneration : IBuilder
    {
        private Room? _room;
        private Room BuildingRoom => _room ?? throw new InvalidOperationException("Room is not initialized.");
        private Random _rng = new Random();

        private BuildResult _result = new BuildResult();

        public IBuilder InitGrid(int rows, int cols)
        {
            _room = new Room(rows, cols, skipGeneration: true);
            return this;
        }

        public IBuilder EmptyDungeon()
        {
            for (int i = 0; i < BuildingRoom.Rows; i++)
            {
                for (int j = 0; j < BuildingRoom.Columns; j++)
                {
                    BuildingRoom.Grid[i, j].IsWall = false;
                }
            }
            return this;
        }

        public IBuilder FilledDungeon()
        {
            for (int i = 0; i < BuildingRoom.Rows; i++)
            {
                for (int j = 0; j < BuildingRoom.Columns; j++)
                {
                    BuildingRoom.Grid[i, j].IsWall = true;
                }
            }
            return this;
        }

        public IBuilder AddChambers()
        {
            int chamberCount = 6;
            for (int c = 0; c < chamberCount; c++)
            {
                int chamberWidth = _rng.Next(3, 8);
                int chamberHeight = _rng.Next(3, 6);
                int row = _rng.Next(1, BuildingRoom.Rows - chamberHeight - 1);
                int col = _rng.Next(1, BuildingRoom.Columns - chamberWidth - 1);
                for (int i = row; i < row + chamberHeight; i++)
                {
                    for (int j = col; j < col + chamberWidth; j++)
                        BuildingRoom.Grid[i, j].IsWall = false;
                }
            }
            return this;
        }

        public IBuilder AddPaths()
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

            return this;
        }

        public IBuilder AddCentralRoom()
        {
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
            return this;
        }

        public IBuilder AddItems()
        {
            PlaceObjectsRandomly(
                3,
                () => Storage.GetRandomItem(),
                (cell, item) => cell.Items.Push(item)
            );
            return this;
        }

        public IBuilder AddWeapons()
        {
            PlaceObjectsRandomly(
                5,
                () => Storage.GetRandomWeapon(),
                (cell, item) => cell.Items.Push(item)
            );
            return this;
        }

        public IBuilder AddPotions()
        {
            PlaceObjectsRandomly(
                12,
                () => Storage.GetRandomPotion(),
                (cell, item) => cell.Items.Push(item)
            );
            return this;
        }

        public IBuilder AddEnemies()
        {
            int enemyCount = 3;
            int attempts = 0;
            while (enemyCount > 0 && attempts < 1000)
            {
                int r = _rng.Next(0, BuildingRoom.Rows);
                int c = _rng.Next(0, BuildingRoom.Columns);
                var cell = BuildingRoom.Grid[r, c];
                if (!cell.IsWall && cell.Items.Count == 0 && !cell.IsPlayerHere && cell.Enemy == null)
                {
                    cell.Enemy = new Models.Enemy("Skeleton", health: 6, attackPower: 3);
                    enemyCount--;
                }
                attempts++;
            }
            return this;
        }

        public IBuilder PlacePlayer(int x, int y)
        {
            BuildingRoom.Grid[x, y].IsWall = false;
            BuildingRoom.PlayerPosition = (x, y);
            BuildingRoom.Grid[x, y].IsPlayerHere = true;
            return this;
        }

        // Expose the final Room object
        public BuildResult GetResult()
        {
            _result.Dungeon = BuildingRoom;
            return _result;
        }

        private void PlaceObjectsRandomly<T>(int count, Func<T> createFunc, Action<Cell, T> placeAction) where T : Models.Item
        {
            int placed = 0;
            int attempts = 0;
            while (placed < count && attempts < 1000)
            {
                int r = _rng.Next(0, BuildingRoom.Rows);
                int c = _rng.Next(0, BuildingRoom.Columns);
                if (!BuildingRoom.Grid[r, c].IsWall && BuildingRoom.Grid[r, c].Enemy == null)
                {
                    placeAction(BuildingRoom.Grid[r, c], createFunc());
                    placed++;
                }
                attempts++;
            }
        }

        public IBuilder EnsureConnectivity()
        {
            // Here is an idea:
            // 1. Collect all the empty cells in the room
            // 2. From the first empty cell use BFS to find all the empty cells that are reachable
            // and all these reachable cells store in the visited set
            // 3. In foreach loop we check if the empty cell is in the visited set:
            // 4. If this cell is not in visited set,
            // then create a path from the nearest visited cell to current unvisited cell

            // 1. Collect all the empty cells in the room
            List<(int, int)> emptyCells = new List<(int, int)>();
            for (int i = 0; i < BuildingRoom.Rows; i++)
            {
                for (int j = 0; j < BuildingRoom.Columns; j++)
                {
                    if (!BuildingRoom.Grid[i, j].IsWall)
                    {
                        emptyCells.Add((i, j));
                    }
                }
            }
            if (emptyCells.Count == 0 || emptyCells.Count == 1)
            {
                return this;
            }

            // Before calling BFS from emptyCells[k], we must ensure that this cell is not a single isolated
            // <=> there is at least one neighbour empty cell (2x1 empty area)
            // Without it there will be no carved path, because visited set will have only one cell 
            // and FindNearestVisited will return the same cell

            int correctCellIndex = 0;

            for (int i = 0; i < emptyCells.Count; i++)
            {
                if (!IsSingleIsolatedCell(emptyCells[i].Item1, emptyCells[i].Item2, BuildingRoom))
                {
                    correctCellIndex = i;
                    break;
                }
            }

            // 2. From the first empty cell use BFS to find all the empty cells that are reachable
            HashSet<(int, int)> visited = BFS(BuildingRoom, emptyCells[0]);

            // 3. In foreach loop we check if the empty cell is in the visited set
            foreach (var (r, c) in emptyCells)
            {
                if (!visited.Contains((r, c)))
                {
                    // 4. Create a path from the nearest visited cell to current unvisited cell
                    var (vr, vc) = FindNearestVisited(r, c, BuildingRoom.Rows, BuildingRoom.Columns, visited);

                    CarveCorridor(BuildingRoom, (r, c), (vr, vc));

                    // Update visited set
                    visited = BFS(BuildingRoom, emptyCells[0]);
                }
            }

            return this;
        }

        private HashSet<(int, int)> BFS(Room room, (int, int) start)
        {
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();

            queue.Enqueue(start);
            visited.Add(start);

            // Declare two arrays that represent directions:
            int[] directionRow = { -1, 1, 0, 0 };
            int[] directionCol = { 0, 0, -1, 1 };
            // i = 0: directionRow[i] and directionCol[i] is move to left.
            // for each i from 0 to 3 move won't be diagonally 


            while (queue.Count > 0)
            {
                var (row, col) = queue.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    // nr - neighbourRow; nc - neighbourCol
                    int nr = row + directionRow[i];
                    int nc = col + directionCol[i];

                    if (nr >= 0 && nr < room.Rows && nc >= 0 && nc < room.Columns)
                    {
                        if (!room.Grid[nr, nc].IsWall && !visited.Contains((nr, nc)))
                        {
                            visited.Add((nr, nc));
                            queue.Enqueue((nr, nc));
                        }
                    }
                }
            }

            return visited;
        }
        private (int, int) FindNearestVisited(int r, int c, int limitR, int limitC, HashSet<(int, int)> visited)
        {
            // If the current cell is visited, just return it
            if (visited.Contains((r, c))) return (r, c);

            // Now we are looking for the 'diagonally' nearest cell
            // Keep in mind the grid. I show these possible closest neighbor coord
            // We will look for the nearest on this area, increasing its radius after each area's check
            //                  (-2,  0),
            //         (-1, -1),( ~,  ~),(-1,  1)
            //( 0, -2),( ~,  ~),( r,  c),( ~,  ~),( 0,  2)
            //         ( 1, -1),( ~,  ~),( 1,  1),
            //                  ( 2,  0)

            int[] directionRow = { -1, 1, 0, 0 };
            int[] directionCol = { 0, 0, -1, 1 };
            int[] diag = { -1, -1, 1, 1 };
            int[] diagI = { -1, 1, 1, -1 };

            // Maximum search distance
            int maxDist = Math.Max(limitR, limitC);
            for (int kd = 1, k = 2; kd <= maxDist || k <= maxDist; kd++, k++)
            {
                // horizontally and vertically neighbour
                if (k <= maxDist)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int nr = r + k * directionRow[i];
                        int nc = c + k * directionCol[i];

                        // Skip out of bounds
                        if (nr >= limitR || nc >= limitC || nr < 0 || nc < 0) continue;
                        if (visited.Contains((nr, nc)))
                        {
                            return (nr, nc);
                        }
                    }
                }



                // diag neighbour
                if (k <= maxDist)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int nr = r + kd * diag[i];
                        int nc = c + kd * diagI[i];

                        // Skip out of bounds
                        if (nr >= limitR || nc >= limitC || nr < 0 || nc < 0) continue;
                        if (visited.Contains((nr, nc)))
                        {
                            return (nr, nc);
                        }
                    }
                }
            }
            return (r, c);
        }

        private void CarveCorridor(Room room, (int, int) start, (int, int) end)
        {
            var (r1, c1) = start;
            var (r2, c2) = end;

            // L-shaped
            while (r1 != r2)
            {
                room.Grid[r1, c1].IsWall = false;
                r1 += r2 > r1 ? 1 : -1;
            }

            while (c1 != c2)
            {
                room.Grid[r1, c1].IsWall = false;
                c1 += c2 > c1 ? 1 : -1;
            }
        }

        private bool IsSingleIsolatedCell(int r, int c, Room room)
        {
            // Check if the cell is isolated
            // If the cell is isolated, then it is impossible to find the nearest visited cell
            // because there is no visited cell
            // So we must ensure that the cell has at least one neighbour empty cell
            // (2x1 empty area)
            int[] directionRow = { -1, 1, 0, 0 };
            int[] directionCol = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nr = r + directionRow[i];
                int nc = c + directionCol[i];

                if (nr >= 0 && nr < room.Rows && nc >= 0 && nc < room.Columns)
                {
                    if (!room.Grid[nr, nc].IsWall)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public IBuilder AddMovement()
        {
            // No movement handler needed in dungeon generation.
            return this;
        }
    }
}
