using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    public sealed class ItemEquippedEvent : BaseEvent
    {
        public int PlayerId { get; set; }
        public string ItemName { get; set; } = "";
        public int HandNumber { get; set; } // 0 = left, 1 = right
    }
}
