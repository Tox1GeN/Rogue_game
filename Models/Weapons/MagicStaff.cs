using Rogue.Models.Combat.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class MagicStaff : Weapon
    {
        public int ManaUsage { get; }
        public bool SplashDamage { get; }
        public int DistanceOfAttack { get; }

        public MagicStaff(string name, int damage, int manaUsage, bool splash, int distance)
        {
            Name = name;
            _damage = damage;
            ManaUsage = manaUsage;
            SplashDamage = splash;
            DistanceOfAttack = distance;
        }

        public override void Accept(IPlayerAttackVisitor visitor) => visitor.VisitMagicWeapon(this);
        public override int Damage => _damage;
    }
}
