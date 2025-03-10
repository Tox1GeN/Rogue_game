using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    internal class ProtectiveEffect : ItemDecorator
    {
        public ProtectiveEffect(Item itemToWrap) : base(itemToWrap) { }

        public override string GetDisplayName() => $"{wrappedItem.GetDisplayName()} (Powerful)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            player.Health += 3;
            Console.WriteLine("Health increased by 3.");
        }

        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Strength -= 3;
            Console.WriteLine("Health reduced by 3");
        }
    }
}
