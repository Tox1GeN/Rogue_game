using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;
using Rogue.UI;

namespace Rogue.Models.UsableItems
{
    public class Potion : Item, IUsable
    {
        public int HealingAmount { get; private set; }
        public Potion(string name, int healingAmount)
        {
            Name = name;
            HealingAmount = healingAmount;
        }

        public void Use(Player player)
        {
            player.Health += HealingAmount;
            Render.Instance.AddActionLine($"You used {Name} and restore helath by {HealingAmount}");
            Render.Instance.FinalizeActionMessage();
        }

        public override string GetDisplayName() => $"{Name} ({HealingAmount})";
    }
}
