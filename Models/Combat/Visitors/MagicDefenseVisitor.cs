using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Shields;

namespace Rogue.Models.Combat.Visitors
{
    public sealed class MagicDefenseVisitor : IDefenseVisitor
    {
        public int TotalDefense { get; private set; }
        public void VisitPlayer(Player p) => TotalDefense += p.Wisdom * 2;
        public void VisitShield(Shield s) => TotalDefense += (int)(s.Armor * 0.5);
    }
}
