using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    internal class UnluckyEffect : ItemDecorator
    {
        public UnluckyEffect(Item itemToWrap) : base(itemToWrap) { }

        public override string GetDisplayName() => $"{wrappedItem.GetDisplayName()} (Unlucky)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            player.Luck -= 5;
            Console.WriteLine("Luck decreased by 5.");
        }

        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Luck += 5;
            Console.WriteLine("Luck restored by 5");
        }
    }
}
