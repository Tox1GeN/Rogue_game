using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;

namespace Rogue.Decorators
{
    internal abstract class ItemDecorator : Item
    {
        protected Item wrappedItem;

        // Constructor of Decorator
        public ItemDecorator(Item itemToWrap)
        {
            wrappedItem = itemToWrap;
        }

        public override string GetDisplayName() => wrappedItem.GetDisplayName();

        public override void Equip(Player player) => wrappedItem.Equip(player);

        public override void Unequip(Player player) => wrappedItem.Unequip(player);
    }
}
