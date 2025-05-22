using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    public abstract class BaseEvent
    {
        public string EventType => GetType().Name;
    }
}
