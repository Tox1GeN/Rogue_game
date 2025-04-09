using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Effects;

namespace Rogue.Models.UsableItems.Potions
{
    public class StrengthPotion : Potion
    {
        private int strengthBoost;

        public StrengthPotion(string name, int strengthBoost, int duration) : base(duration)
        {
            Name = name;
            this.strengthBoost = strengthBoost;
        }

        public override void Use(Player player)
        {
            var effect = new StrengthBoostEffect(strengthBoost, duration);
            player.AttachEffect(effect);
            effect.Apply(player);
        }
    }
}
