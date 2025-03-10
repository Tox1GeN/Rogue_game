using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal class WeaponItem : Item
    {
        public int Damage { get; set; }
        public bool TwoHanded { get; set; }

        public string ShortReview { get; set; }

        public string SecretMessage { get; set; }

        public WeaponItem(string name, int damage, bool twoHanded)
        {
            Name = name;
            Damage = damage;
            TwoHanded = twoHanded;
        }

        public override string GetDisplayName() => Name; // ??? decorate ???

        public override HandRequirement Requirement => TwoHanded ? HandRequirement.Two : HandRequirement.One;

        public override void Equip(Player player)
        {
            // In decorators change a value of a player characteristic here
            Console.WriteLine($"You've equipped the \"{GetDisplayName()}\". {ShortReview}");
        }

        public override void Unequip(Player player)
        {
            // In decorators undo the changes of the value of the player characteristic here
            Console.WriteLine($"You've unequipped the \"{GetDisplayName()}\". {SecretMessage}");
        }
    }
}
