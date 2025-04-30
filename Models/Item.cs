using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Models.Combat.Visitors;
using Rogue.Models.Interfaces;
using Rogue.Models.Weapons;

namespace Rogue.Models
{
    public abstract class Item : IItem
    {
        public virtual string Name { get; protected set; }
        public virtual string GetDisplayName() => Name;
        public virtual bool CanEquip => false;
        public virtual bool CanUse => false;
        public virtual bool TwoHanded => false;
        public virtual void Equip(Player player) { }
        public virtual void Unequip(Player player) { }
        public virtual void PickUp(Player player, Room currentRoom) { }
        public virtual void Use(Player player) { }
        public virtual Item TryEnchant(Random rng) => this;
        public virtual void Accept(IDefenseVisitor visitor) { }
        public virtual Weapon? AsWeapon() => null;
    }
}
