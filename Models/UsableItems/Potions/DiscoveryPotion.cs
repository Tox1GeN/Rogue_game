using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Rogue.Models.Effects;

namespace Rogue.Models.UsableItems.Potions
{
    public class DiscoveryPotion : Potion
    {
        private int wisdomIncrease;

        public DiscoveryPotion(string name, int wisdomIncrease, int duration) : base(duration)
        {
            Name = name;
            this.wisdomIncrease = wisdomIncrease;
        }

        public override void Use(Player player)
        {
            var effect = new WisdomBoostEffect(wisdomIncrease, duration);
            player.AttachEffect(effect);
            effect.Apply(player);
        }
    }
}
