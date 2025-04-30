using Rogue.Models.Combat.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class Bow : Weapon
    {
        public int PiercingStrength { get; }
        public int DistanceOfAttack { get; }

        public Bow(string name, int damage, int pierce, int distance)
        {
            Name = name;
            _damage = damage;
            PiercingStrength = pierce;
            DistanceOfAttack = distance;
        }

        public override void Accept(IPlayerAttackVisitor visitor) => visitor.VisitLightWeapon(this);
        public override int Damage => _damage;
    }
}