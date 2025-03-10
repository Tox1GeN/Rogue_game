using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    internal class PowerfulEffect : ItemDecorator
    {
        public PowerfulEffect(Item itemToWrap) : base(itemToWrap) { }

        public override string GetDisplayName() => $"{wrappedItem.GetDisplayName()} (Powerful)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            player.Strength += 2;
            Console.WriteLine("Strength increased by 2.");
        }

        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Strength -= 2;
            Console.WriteLine("Strength reduced by 2");
        }
    }
}
