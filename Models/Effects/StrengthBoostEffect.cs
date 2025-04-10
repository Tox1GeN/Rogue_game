using Rogue.Models.Interfaces;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Effects
{
    public class StrengthBoostEffect : IEffect
    {
        private bool isPermanent;
        private int duration;
        private int strengthBoost;

        public bool IsExpired => duration <= 0;

        public StrengthBoostEffect(int strengthBoost, int duration = -1)
        {
            this.strengthBoost = strengthBoost;
            this.duration = duration;
            isPermanent = duration <= 0;
        }

        public void Apply(Player player)
        {
            player.Strength += strengthBoost;
            Render.Instance.AddActionLine($"You feel stronger! Your strength increased by {strengthBoost}.");
        }


        public void OnTurnPassed(Player player)
        {
            if (duration > 0)
            {
                duration--;
            }
            if (duration == 0)
            {
                OnExpire(player);
                player.DetachEffect(this);
                
                //Render.Instance.StartNewActionMessage();
                //Render.Instance.AddActionLine("Your strength boost has worn off.");
                //Render.Instance.FinalizeActionMessage();
            }
        }

        public void OnExpire(Player player)
        {
            player.Strength -= strengthBoost;
        }
    }
}
