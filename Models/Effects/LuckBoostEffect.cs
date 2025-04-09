using Rogue.Models.Interfaces;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Effects
{
    public class LuckBoostEffect : IEffect
    {
        private bool isPermanent;
        private int multiplier;
        private int originalLuck;
        private int duration;

        public bool IsExpired => !isPermanent && duration <= 0;

        public LuckBoostEffect(int multiplier, int duration = -1)
        {
            this.duration = duration;
            this.multiplier = multiplier;
            this.isPermanent = duration <= 0;
        }

        public void Apply(Player player)
        {
            // Store original luck to revert later
            originalLuck = player.Luck;

            // On the 1st turn, i = 1 -> multiplier = n - 1 + 1 = n.
            player.Luck = originalLuck * multiplier;
            Render.Instance.AddActionLine($"Luck multiplied by {multiplier} for {duration} turns!");
        }

        public void OnTurnPassed(Player player)
        {
            if (isPermanent)
                return;

            if (duration > 0)
                duration--;
            
            if ( duration > 0)
            {
                player.Luck = originalLuck * duration;
            }
            if (duration == 0)
            {
                // Revert luck back to original
                player.Luck = originalLuck;
                player.DetachEffect(this);
                
                //Render.Instance.StartNewActionMessage();
                //Render.Instance.AddActionLine("The luck boost has ended.");
                //Render.Instance.FinalizeActionMessage();
            }
        }
    }
}
