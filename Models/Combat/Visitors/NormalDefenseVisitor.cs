using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Shields;

namespace Rogue.Models.Combat.Visitors
{
    public sealed class NormalDefenseVisitor : IDefenseVisitor
    {
        public int TotalDefense { get; private set; }
        public void VisitPlayer(Player p) => TotalDefense += p.Dexterity + p.Luck;
        public void VisitShield(Shield s) => TotalDefense += s.Armor;
    }
}