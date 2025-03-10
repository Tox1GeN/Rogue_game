using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal enum HandRequirement
    {
        None,  // For items that cannot be equipped
        One,   // For one-handed items
        Two    // For two-handed items
    }
    internal class Item
    {
        public virtual string Name { get; protected set; }
        public virtual string GetDisplayName() => Name;
        public virtual void Equip(Player player) { }
        public virtual void Unequip(Player player) { }

        // By default the item is unequipable. Override it in a child class
        public virtual HandRequirement Requirement => HandRequirement.None;
    }
}
