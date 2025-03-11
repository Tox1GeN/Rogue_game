using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    internal class Sword : WeaponItem
    {
        public bool TwoHanded { get; private set; }
        public Sword(string name, int damage, bool twoHanded) : base(name, damage)
        {
            TwoHanded = twoHanded;
        }
        public override HandRequirement Requirement => TwoHanded ? HandRequirement.Two : HandRequirement.One;
    }
}
