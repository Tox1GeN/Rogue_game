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
            MessageBuffer.Begin();
            MessageBuffer.Add("Nothing happens... (Invalid key pressed)");
            MessageBuffer.Commit();
            // No next handler to call because this is the end of the chain.
        }
    }
}
