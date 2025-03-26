using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class DungeonDirector
    {
        private IDungeonBuilder _builder;

        public DungeonDirector(IDungeonBuilder builder)
        {
            _builder = builder;
        }

        // First Level ???
        public Room ConstructDefaultDungeon(int rows = 20, int cols = 40)
        {
            _builder.InitGrid(rows, cols);
            _builder.FilledDungeon();
            _builder.AddPaths();
            _builder.AddChambers();
            //_builder.AddCentralRoom();
            _builder.AddItems();
            _builder.AddWeapons();
            _builder.PlacePlayer(6, 4);
            EnsureConectivity();
            return _builder.GetResult();
        }

        // Test (Empty Dungeon)

        //public Room ConstructEmptyDungeon(int rows = 20, int cols = 40)
        //{
        //    _builder.InitGrid(rows, cols);
        //    _builder.PlacePlayer();
        //    return _builder.GetResult();
        //}

        //public Room ConstructDungeonWithRoomsAndPaths(int rows = 20, int cols = 40)
        //{
        //    _builder.InitGrid(rows, cols);
        //    _builder.BuildWalls();
        //    _builder.AddCentralRoom();
        //    _builder.AddItems();
        //    _builder.AddWeapons();
        //    _builder.PlacePlayer();
        //    return _builder.GetResult();
        //}

        public void EnsureConectivity()
        {
            Room checkingRoom = _builder.GetResult();

            // Here is an idea:
            // 1. Collect all the empty cells in the room
            // 2. From the first empty cell use BFS to find all the empty cells that are reachable
            // and all these reachable cells store in the visited set
            // 3. In foreach loop we check if the empty cell is in the visited set:
            // 4. If this cell is not in visited set,
            // then create a path from the nearest visited cell to current unvisited cell

            // 1. Collect all the empty cells in the room
            List<(int, int)> emptyCells = new List<(int, int)>();
            for (int i = 0; i < checkingRoom.Rows; i++)
            {
                for (int j = 0; j < checkingRoom.Columns; j++)
                {
                    if (!checkingRoom.Grid[i, j].IsWall)
                    {
                        emptyCells.Add((i, j));
                    }
                }
            }
            if (emptyCells.Count == 0)
            {
                return;
            }

            // Before calling BFS from emptyCells[k], we must ensure that this cell is not a single isolated
            // <=> there is at least one neighbour empty cell (2x1 empty area)
            // Without it there will be no carved path, because visited set will have only one cell 
            // and FindNearestVisited will return the same cell

            int correctCellIndex = 0;

            for(int i = 0; i < emptyCells.Count; i++)
            {
                if (!IsSingleIsolatedCell(emptyCells[i].Item1, emptyCells[i].Item2, checkingRoom))
                {
                    correctCellIndex = i;
                    break;
                }
            }


            // 2. From the first empty cell use BFS to find all the empty cells that are reachable
            HashSet<(int, int)> visited = BFS(checkingRoom, emptyCells[correctCellIndex]);

            // 3. In foreach loop we check if the empty cell is in the visited set
            foreach (var (r, c) in emptyCells)
            {
                if(!visited.Contains((r, c)))
                {
                    // 4. Create a path from the nearest visited cell to current unvisited cell
                    var (vr, vc) = FindNearestVisited(r, c, checkingRoom.Rows, checkingRoom.Columns, visited);

                    //
                    if ((vr, vc) == (r, c))
                    {
                        Console.WriteLine("Error: There is only one empty cell");
                        return;
                    }

                    CarveCorridor(checkingRoom, (r, c), (vr, vc));


                    // Update visited set
                    visited = BFS(checkingRoom, emptyCells[0]);
                }
            }


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

                    if(nr >= 0 && nr < room.Rows && nc >= 0 && nc < room.Columns)
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

            int[] directionRow  = { -1,  1,  0,  0 };
            int[] directionCol  = {  0,  0, -1,  1 };
            int[] diag          = { -1, -1,  1,  1 };
            int[] diagI         = { -1,  1,  1, -1 };

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
            while(r1 != r2)
            {
                room.Grid[r1, c1].IsWall = false;
                r1 += (r2 > r1) ? 1 : -1;
            }

            while (c1 != c2)
            {
                room.Grid[r1, c1].IsWall = false;
                c1 += (c2 > c1) ? 1 : -1;
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
    }
}
