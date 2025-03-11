using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UnusableItems
{
    public class MysteriousNote : UnusableItem
    {
        public string PartOfLore { get; set; }
        public MysteriousNote(string name, string shortDesc, string lorePart) : base(name, shortDesc)
        {
            PartOfLore = lorePart;
        }
    }
}
