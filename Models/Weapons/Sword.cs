using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class Sword : Weapon
    {
        public Sword(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }

        public override string GetDisplayDmg () => base.GetDisplayDmg();
    }
}
