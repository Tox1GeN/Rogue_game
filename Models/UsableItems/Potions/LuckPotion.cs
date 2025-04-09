using Rogue.Models.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UsableItems.Potions
{
    public class LuckPotion : Potion
    {
        private int multiplier;

        public LuckPotion(string name, int multiplier, int duration) : base(duration)
        {
            Name = name;
            this.multiplier = multiplier;
        }

        public override void Use(Player player)
        {
            var effect = new LuckBoostEffect(multiplier, duration);
            player.AttachEffect(effect);
            effect.Apply(player);
        }
    }
}
