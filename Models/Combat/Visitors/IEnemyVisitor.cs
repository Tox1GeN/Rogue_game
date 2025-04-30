using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Combat.Visitors
{
    public interface IEnemyVisitor
    {
        void VisitEnemy(Enemy enemy);
    }
}