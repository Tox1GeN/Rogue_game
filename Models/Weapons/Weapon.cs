using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public class WeaponItem : Equipment
    {
        public int Damage { get; private set; }

        // The future idea to add short commet on equip action. For Example:
        // You've equipped Excalibur. You fill Blessed
        // public string ShortReview { get; private set; }

        // One more similar idea, but for unequip.
        // You've unequipped the Curse Sword. You've not felt better.
        // public string SecretMessage { get; private set; }

        public WeaponItem(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }

        public override void ModifyDamage(int buffOrNerf)
        {
            Damage += buffOrNerf;
        }
    }
}
