using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    public abstract class Item : IItem
    {
        public virtual string Name { get; protected set; }
        public virtual string GetDisplayName() => Name;
        public virtual void Equip(Player player) { }
        public virtual void Unequip(Player player) { }
    }
}
