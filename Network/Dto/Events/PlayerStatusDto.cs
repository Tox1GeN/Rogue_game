using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    public sealed class PlayerStatusDto : BaseEvent
    {
        public int Id { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Luck { get; set; }
        public int Wisdom { get; set; }

        // TODO: Fix the Coin and Gold classes
        // public int Coins { get; set; }
        public int InventoryCount { get; set; }

        /// <summary>Display names of items currently in hands (left->right).</summary>
        public List<string> Equipped { get; set; } = new();
    }
}
