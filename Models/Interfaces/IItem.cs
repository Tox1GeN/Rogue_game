using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    public interface IItem
    {
        string Name { get; }
        string GetDisplayName();
        bool CanEquip { get; }
        bool TwoHanded { get; }
        void Equip(Player player) { }
        void Unequip(Player player) { }
    }
}
