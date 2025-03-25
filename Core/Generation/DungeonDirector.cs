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
            _builder.BuildWalls();
            _builder.AddCentralRoom();
            _builder.AddItems();
            _builder.AddWeapons();
            _builder.PlacePlayer();
            return _builder.GetResult();
        }

        // Test (Empty Dungeon)

        public Room ConstructEmptyDungeon(int rows = 20, int cols = 40)
        {
            _builder.InitGrid(rows, cols);
            _builder.PlacePlayer();
            return _builder.GetResult();
        }

        public Room ConstructDungeonWithRoomsAndPaths(int rows = 20, int cols = 40)
        {
            _builder.InitGrid(rows, cols);
            _builder.BuildWalls();
            _builder.AddCentralRoom();
            _builder.AddItems();
            _builder.AddWeapons();
            _builder.PlacePlayer();
            return _builder.GetResult();
        }
    }
}
