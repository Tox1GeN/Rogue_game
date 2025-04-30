using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Combat.Visitors;
using Rogue.Models.Shields;

namespace Rogue.Models.Combat.Visitors
{
    public interface IDefenseVisitor
    {
        void VisitPlayer(Player player);
        void VisitShield(Shield shield);
        int TotalDefense { get; }
    }
}
