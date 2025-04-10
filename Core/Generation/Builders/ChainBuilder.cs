using Rogue.Core.Generation.Interfaces;
using Rogue.UI.Input_Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation.Builders
{
    public class ChainBuilder : IBuilder
    {
        private List<InputHandler> _handlers = new List<InputHandler>();
        private BuildResult _result = new BuildResult();

        private bool pickAndDropHandlerAdded = false;

        public IBuilder InitGrid(int rows, int cols) { return this; }
        public IBuilder EmptyDungeon() { return this; }
        public IBuilder FilledDungeon() { return this; }
        public IBuilder AddChambers() { return this; }
        public IBuilder AddPaths() { return this; }
        public IBuilder AddCentralRoom() { return this; }

        public IBuilder AddMovement()
        {
            _handlers.Add(new MovementHandler());
            return this;
        }

        public IBuilder AddItems()
        {
            if (!pickAndDropHandlerAdded)
            {
                _handlers.Add(new PickupHandler());
                _handlers.Add(new DropHandler());
                pickAndDropHandlerAdded = true;
            }
            return this;
        }

        public IBuilder AddWeapons()
        {
            if (!pickAndDropHandlerAdded)
            {
                _handlers.Add(new PickupHandler());
                _handlers.Add(new DropHandler());
                pickAndDropHandlerAdded = true;
            }
            _handlers.Add(new EquipHandler());
            _handlers.Add(new UnequipHandler());
            return this;
        }

        public IBuilder AddPotions()
        {
            if (!pickAndDropHandlerAdded)
            {
                _handlers.Add(new PickupHandler());
                _handlers.Add(new DropHandler());
                pickAndDropHandlerAdded = true;
            }
            _handlers.Add(new UseHandler());
            return this;
        }

        public IBuilder AddEnemies() { return this; }
        public IBuilder PlacePlayer(int x, int y) { return this; }
        public IBuilder EnsureConnectivity() { return this; }

        // When we get the result, automatically link the handlers into one chain.
        public BuildResult GetResult()
        {
            // Always append an ExitHandler and a DefaultHandler at the end.
            _handlers.Add(new ExitHandler());            
            _handlers.Add(new DefaultHandler());

            // Link the handlers so that each handler's Next pointer points to the next in the list.
            for (int i = 0; i < _handlers.Count - 1; i++)
            {
                _handlers[i].SetNext(_handlers[i + 1]);
            }
            _result.InputHandlers.AddRange(_handlers);
            return _result;
        }
    }
}
