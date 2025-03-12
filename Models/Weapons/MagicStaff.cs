using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class MagicStaff : Weapon
    {
        public int ManaUsage { get; private set; }
        public bool SplashDamage { get; private set; }
        public int DistanceOfAttack { get; private set; }

        public MagicStaff(string name, int damage, int manaUsage, bool splashDmg, int distance)
        {
            Name = name;
            Damage = damage;
            ManaUsage = manaUsage;
            SplashDamage = splashDmg;
            DistanceOfAttack = distance;
        }
    }
}
