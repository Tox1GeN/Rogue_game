using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;
using Rogue.UI;

namespace Rogue.Models.UsableItems.Potions
{
    public abstract class Potion : Item
    {
        public override bool CanUse => true;

        protected int duration;

        public Potion(int duration)
        {
            this.duration = duration;
        } 
    }
}
