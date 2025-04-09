using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Core.Generation.Interfaces;

namespace Rogue.Core.Generation
{
    public class DungeonDirector
    {
        private readonly IBuilder _builder;

        public DungeonDirector(IBuilder builder)
        {
            _builder = builder;
        }

        public BuildResult ConstructDungeon(int rows, int cols, int playerX, int playerY)
        {
            _builder.InitGrid(rows, cols)
                    .FilledDungeon()
                    .AddCentralRoom()
                    .AddChambers()
                    .AddPaths()
                    .AddItems()
                    .AddWeapons()
                    .AddPotions()
                    .AddEnemies()
                    .AddMovement()
                    .PlacePlayer(playerX, playerY)
                    .EnsureConnectivity();
            return _builder.GetResult();
        }
    }
}


//public Room ConstructFirstLevelDungeon(int rows = 20, int cols = 40)
//{
//    _builder.InitGrid(rows, cols);
//    _builder.FilledDungeon();
//    _builder.AddCentralRoom();
//    _builder.AddChambers();
//    _builder.AddPotions();
//    _builder.PlacePlayer(17, 15);
//    _builder.EnsureConectivity();
//    return _builder.GetResult();
//}

//public Room ConstructSecondLevelDungeon(int rows = 20, int cols = 40)
//{
//    _builder.InitGrid(rows, cols);
//    _builder.FilledDungeon();
//    _builder.AddPaths();
//    _builder.AddItems();
//    _builder.AddWeapons();
//    _builder.PlacePlayer(3, 4);
//    _builder.EnsureConectivity();
//    return _builder.GetResult();
//}

//public Room ConstructThirdLevelDungeon(int rows = 20, int cols = 40)
//{
//    _builder.InitGrid(rows, cols);
//    _builder.FilledDungeon();
//    _builder.PlacePlayer(3, 4);
//    _builder.AddPaths();
//    _builder.AddItems();
//    _builder.AddWeapons();
//    _builder.AddPotions();
//    _builder.AddEnemies();
//    _builder.EnsureConectivity();
//    return _builder.GetResult();
//}