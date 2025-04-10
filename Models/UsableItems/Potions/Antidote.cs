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
                Render.Instance.StartNewActionMessage();
                Render.Instance.AddActionLine("You feel fine. There are no active effects to remove.");
                Render.Instance.FinalizeActionMessage();
                return;
            }

            // Remove all effects – or you can filter to remove only negative ones if you prefer
            foreach (var effect in effects.ToList())
            {
                effect.OnExpire(player);
                player.DetachEffect(effect);            }
                

            Render.Instance.StartNewActionMessage();
            Render.Instance.AddActionLine("All eff removed by the antidote!");
            Render.Instance.FinalizeActionMessage();
        }
    }
}
