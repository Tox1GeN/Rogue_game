using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Weapons;

namespace Rogue.Models.Combat.Visitors
{
    public sealed class StealthAttackVisitor : IPlayerAttackVisitor
    {
        private readonly Player _p;
        public int Damage { get; private set; }
        public StealthAttackVisitor(Player p) => _p = p;
        public void VisitHeavyWeapon(Weapon w)
            => Damage = (int)((w.Damage + _p.Strength + _p.Aggression) * 0.5);
        public void VisitLightWeapon(Weapon w)
            => Damage = (w.Damage + _p.Dexterity + _p.Luck) * 2;
        public void VisitMagicWeapon(Weapon w)
            => Damage = w.Damage + _p.Wisdom;
    }
}