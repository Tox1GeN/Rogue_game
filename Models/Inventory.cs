using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.UI;

namespace Rogue.Models
{
    public class Inventory
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
            {
                MessageBuffer.Add($"Your Inventory is Full!");
                MessageBuffer.Commit();
                return false;
            }
            
            //Otherwise add this item
            Items.Add(pickup);
            return true;
        }

        public void RemoveItemAt(int inventoryIndex)
        {
            // Can't remove something not existing
            if (inventoryIndex < 0 || inventoryIndex >= Items.Count)
                return;

            // Otherwise remove out from 'bag' this item
            Items.RemoveAt(inventoryIndex);
        }

        public Item? ItemAt(int inventoryIndex)
        {
            // Can't get something not existing
            if (inventoryIndex < 0 || inventoryIndex >= Items.Count)
                return null;

            // Otherwise return it
            return Items[inventoryIndex];
        }
    }
}
