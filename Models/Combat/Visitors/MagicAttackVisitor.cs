using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Weapons;

namespace Rogue.Models.Combat.Visitors
{
    public sealed class MagicAttackVisitor : IPlayerAttackVisitor
    {
        private readonly Player _p;
        public int Damage { get; private set; }
        public MagicAttackVisitor(Player p) => _p = p;
        public void VisitHeavyWeapon(Weapon w)
            => Damage = 1;   // heavy weapon in magic attack deals 1
        public void VisitLightWeapon(Weapon w)
            => Damage = 1;   // light weapon in magic attack deals 1
        public void VisitMagicWeapon(Weapon w)
            => Damage = w.Damage + _p.Wisdom;
    }
}