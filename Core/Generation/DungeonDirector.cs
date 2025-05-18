using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Core.Generation.Interfaces;
using Rogue.Models;

namespace Rogue.Core.Generation
{
    public class DungeonDirector
    {
        private readonly IBuilder _builder;

        public DungeonDirector(IBuilder builder)
        {
            _builder = builder;
        }

        public BuildResult ConstructDungeon(int rows, int cols, Player player, int playerX, int playerY)
        {
            _builder.InitGrid(rows, cols)
                    .FilledDungeon()
                    //.AddCentralRoom()
                    .AddChambers()
                    .AddPaths()
                    .AddItems()
                    .AddWeapons()
                    .AddPotions()
                    .AddEnemies()
                    .AddMovement()
                    .PlacePlayer(player, playerX, playerY)
                    .EnsureConnectivity();
            return _builder.GetResult();
        }
    }
}