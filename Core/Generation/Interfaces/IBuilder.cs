using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation.Interfaces
{
    public interface IBuilder
    {
        IBuilder InitGrid(int rows, int cols);
        IBuilder EmptyDungeon();
        IBuilder FilledDungeon();
        IBuilder AddChambers();
        IBuilder AddPaths();
        IBuilder AddCentralRoom();
        IBuilder AddItems();
        IBuilder AddWeapons();
        IBuilder AddPotions();
        IBuilder AddEnemies();
        IBuilder AddMovement();
        IBuilder PlacePlayer(int x, int y);
        IBuilder EnsureConnectivity();
        BuildResult GetResult();
    }

}
