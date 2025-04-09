using Rogue.Models.Interfaces;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Effects
{
    public class WisdomBoostEffect : IEffect
    {
        private bool isPermanent;
        private int wisdomIncrease;
        private int duration;

        public bool IsExpired => !isPermanent && duration <= 0;

        public WisdomBoostEffect(int wisdomIncrease, int duration = -1)
        {
            this.wisdomIncrease = wisdomIncrease;
            this.duration = duration;
            isPermanent = duration <= 0;            
        }

        public void Apply(Player player)
        {
            player.Wisdom += wisdomIncrease;
            Render.Instance.AddActionLine($"Your wisdom increased by {wisdomIncrease}!");
        }

        public void OnTurnPassed(Player player)
        {
            if (isPermanent)
                return;

            if (duration > 0)
                duration--;

            if (duration == 0)
            {
                player.Wisdom -= wisdomIncrease;
                player.DetachEffect(this);

                //Render.Instance.StartNewActionMessage();
                //Render.Instance.AddActionLine("Your wisdom boost has worn off.");
                //Render.Instance.FinalizeActionMessage();
            }
        }
    }
}
