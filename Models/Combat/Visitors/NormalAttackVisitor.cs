using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Weapons;

namespace Rogue.Models.Combat.Visitors
{
    public sealed class NormalAttackVisitor : IPlayerAttackVisitor
    {
        private readonly Player _p;
        public int Damage { get; private set; }
        public NormalAttackVisitor(Player p) => _p = p;
        public void VisitHeavyWeapon(Weapon w) => Damage = w.Damage + _p.Strength + _p.Aggression;
        public void VisitLightWeapon(Weapon w) => Damage = w.Damage + _p.Dexterity + _p.Luck;
        public void VisitMagicWeapon(Weapon w) => Damage = w.Damage + _p.Wisdom;
    }
}