using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public interface IDungeonBuilder
    {
        void InitGrid(int rows, int cols);
        void EmptyDungeon();
        void FilledDungeon();
        void AddChambers();
        void AddPaths();
        void AddCentralRoom();
        void AddItems();
        void AddWeapons();
        void AddModifiedWeapons();
        void AddEnemies();
        void PlacePlayer(int x, int y);
        Room GetResult();       
    }
}
