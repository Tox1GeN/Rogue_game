using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    public interface IItem
    {
        string Name { get; }
        string GetDisplayName();

        void Equip(Player player);
        void Unequip(Player player);
    }
}

