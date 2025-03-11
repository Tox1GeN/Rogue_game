using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.UnusableItems
{
    public class Rubbish : UnusableItem
    {
        public Rubbish(string name = "Rubbish", string desc = "It's stinks. And you too now.") : base(name, desc) { }
    }
}
