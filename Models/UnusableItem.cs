using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal class UnusableItem: Item
    {
        public string Descripton { get; set; }
        public UnusableItem(string name, string desc)
        {
            Name = name;
            Descripton = desc;
        }

        public override string GetDisplayName() => Name;
        public string GetDisplayDescription() => Descripton;
    }
}
