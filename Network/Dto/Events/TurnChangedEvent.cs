using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    public sealed class TurnChangedEvent : BaseEvent
    {
        public int PlayerId { get; set; }
        public int MovesRemaining { get; set; }
    }
}
