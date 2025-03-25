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
        void BuildWalls();
        void AddCentralRoom();
        void AddItems();
        void AddWeapons();
        void PlacePlayer();
        Room GetResult();       
    }
}
