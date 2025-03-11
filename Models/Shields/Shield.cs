using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Shields
{
    public class ShieldItem : Equipment
    {
        public int ArmorBonus { get; private set; }
        public int ParryDamage { get; private set; }
        public int MiliSecondsToParry { get; private set; }

        // The future idea to add short commet on equip action. For Example:
        // You've equipped Excalibur. You fill Blessed
        // public string ShortReview { get; set; }

        // One more similar idea, but for unequip.
        // You've unequipped the Curse Sword. You've not felt better.
        // public string SecretMessage { get; set; }

        public ShieldItem(int armor, int parryDmg, int timeParry)
        {
            ArmorBonus = armor;
            ParryDamage = parryDmg;
            MiliSecondsToParry = timeParry;
        }
    }
}
