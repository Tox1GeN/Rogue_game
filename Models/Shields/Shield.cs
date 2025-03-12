using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;

namespace Rogue.Models.Shields
{
    public class Shield : Equipment, IShield
    {
        public int Armor { get; private set; }
        public int ParryDamage { get; private set; }
        public int MilliSecondsToParry { get; private set; }

        // The future idea to add short commet on equip action. For Example:
        // You've equipped Excalibur. You fill Blessed
        // public string ShortReview { get; set; }

        // One more similar idea, but for unequip.
        // You've unequipped the Curse Sword. You've not felt better.
        // public string SecretMessage { get; set; }

        public Shield(int armor, int parryDmg, int timeParry)
        {
            Armor = armor;
            ParryDamage = parryDmg;
            MilliSecondsToParry = timeParry;
        }
    }
}
