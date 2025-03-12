using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons.TwoHanded
{
    public abstract class TwoHandedWeapon : Weapon
    {
        public override bool TwoHanded => true;
    }
}
