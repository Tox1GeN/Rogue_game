using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    public interface IUsable : IItem
    {
        void Use(Player player);
    }
}
