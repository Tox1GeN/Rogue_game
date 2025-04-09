using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI.Input_Handlers
{
    public class DefaultHandler : InputHandler
    {
        public override void Handle(ConsoleKey key, Player player, Core.Room currentRoom)
        {
            // This will catch any key that wasn't handled by previous handlers
            Render.Instance.StartNewActionMessage();
            Render.Instance.AddActionLine("Nothing happens... (Invalid key pressed)");
            Render.Instance.FinalizeActionMessage();
            // No next handler to call because this is the end of the chain.
        }
    }
}
