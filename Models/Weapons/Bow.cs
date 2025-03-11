using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class Bow : WeaponItem
    {

        public int PiercingStrength { get; private set; }
        public int DistanceOfAttack { get; private set; }

        public Bow(string name, int damage, int pierce, int distance) : base(name, damage)
        {
            PiercingStrength = pierce;
            DistanceOfAttack = distance;
        }
    }
}
