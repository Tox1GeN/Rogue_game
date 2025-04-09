using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    public interface IEffect
    {
        void OnTurnPassed(Player player);
        bool IsExpired { get; }
        void Apply(Player player);
    }
}
