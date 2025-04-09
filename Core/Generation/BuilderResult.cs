using Rogue.UI.Input_Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class BuildResult
    {
        public Room? Dungeon { get; set; }
        public List<string> Instructions { get; set; } = new List<string>();
        public List<InputHandler> InputHandlers { get; set; } = new List<InputHandler>();
    }
}
