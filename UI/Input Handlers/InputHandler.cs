using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Models;

namespace Rogue.UI.Input_Handlers
{
    public class InputHandler
    {

        protected InputHandler? nextHandler;


        // Linking to the chain
        public InputHandler SetNext(InputHandler nextHandler)
        {
            this.nextHandler = nextHandler;
            return nextHandler;
        }

        public virtual void Handle(ConsoleKey key, Player player, Room currentRoom)
        {
            if (nextHandler != null)
            {
                nextHandler.Handle(key, player, currentRoom);
            }
            // If no nextHandler, the input is unhandled and just drops (or we could log it).
        }
    }
}
