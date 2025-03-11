using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UnusableItems
{
    internal class UnusableItem : Item
    {
        public string ShortDescription { get; set; }
        public UnusableItem(string name, string desc)
        {
            Name = name;
            ShortDescription = desc;
        }

        public override string GetDisplayName() => Name;
        public string GetDisplayDescription() => ShortDescription;
    }
}
