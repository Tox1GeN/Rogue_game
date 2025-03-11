using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal abstract class Equipment : Item
    {
        public override string GetDisplayName() => Name;

        public override void Equip(Player player)
        {
            // In decorators change a value of a player characteristic here
            Console.WriteLine($"You've equipped the \"{GetDisplayName()}\"."); // {ShortReview}");
        }

        public override void Unequip(Player player)
        {
            // In decorators undo the changes of the value of the player characteristic here
            Console.WriteLine($"You've unequipped the \"{GetDisplayName()}\"."); // {SecretMessage}");
        }
    }
}
