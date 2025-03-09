using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal class Inventory
    {
        // Restrict outside modification (cheating)
        public List<Item> Items { get; private set; }

        public int Capacity { get; private set; }

        // There is always an opportiunity to upgrade the capacity
        public Inventory(int currentCapacity = 7)
        {
            Items = new List<Item>();
            Capacity = currentCapacity;
        }

        public bool AddItem(Item pickup)
        {
            // Can't store an item in the full inventory
            if (Items.Count == Capacity)
                return false;
            
            //Otherwise add this item
            Items.Add(pickup);
            return true;
        }

        public Item? LayOutOfInventoryAt(int inventoryIndex)
        {
            // Can't remove something not existing
            if (inventoryIndex < 0 || inventoryIndex >= Items.Count)
                return null;

            // Otherwise lay out from 'bag' this item
            Item dropItem = Items[inventoryIndex];
            Items.RemoveAt(inventoryIndex);
            return dropItem;
        }
    }
}
