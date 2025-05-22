using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network
{
    public sealed class PendingCombat
    {
        public Player Attacker { get; init; }
        public Enemy Target { get; init; }
        public int DestRow { get; init; }
        public int DestCol { get; init; }
    }
}
