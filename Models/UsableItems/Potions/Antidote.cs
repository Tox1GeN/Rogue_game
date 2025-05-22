using Rogue.Models.Interfaces;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UsableItems.Potions
{
    public class Antidote : Item
    {
        public Antidote()
        {
            Name = "Panacea";
        }
        public override bool CanUse => true;
        public override void Use(Player player)
        {
            var effects = player.GetActiveEffects();
            if (effects.Count == 0)
            {
                MessageBuffer.Begin();
                MessageBuffer.Add("You feel fine. There are no active effects to remove.");
                MessageBuffer.Commit();
                return;
            }

            foreach (var effect in effects.ToList())
            {
                effect.OnExpire(player);
                player.DetachEffect(effect);
            }

            MessageBuffer.Begin();
            MessageBuffer.Add("All eff removed by the antidote!");
            MessageBuffer.Commit();
        }
    }
}
