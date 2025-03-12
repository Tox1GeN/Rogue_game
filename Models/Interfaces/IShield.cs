using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    public interface IShield
    {
        int Armor { get; }
        int ParryDamage { get; }
        int MilliSecondsToParry { get; }
    }
}
