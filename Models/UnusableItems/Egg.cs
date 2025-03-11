using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UnusableItems
{
    internal class Egg : UnusableItem
    {
        public Egg(string name = "EASTER EGG", string desc = "???") : base(name, desc) { }
    }
}
