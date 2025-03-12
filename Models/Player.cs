using Rogue.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    public class Player 
    {
        //Player's characteristics
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Health { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }

        // Player has inventory
        public Inventory Inventory { get; set; }

        // Hands Logic
        public Item?[] Hands { get; private set; }

        // Constructor for the default player:
        public Player()
        {
            Strength = 1;
            Dexterity = 1;
            Health = 10;
            Luck = 0;
            Aggression = 0;
            Wisdom = 0;
            Inventory = new Inventory();
            Hands = new Item?[2]; // index 0: left hand, index 1: right hand
        }

        // Player Actions
        public void Move (/*future parameteres*/)
        {
            // TODO: Move implementation
        }

        public bool PickupItem(Room currentRoom)
        {
            if (currentRoom == null)
                return false;

            (int row_X, int col_Y) = currentRoom.PlayerPosition;

            Item? pickup = currentRoom.RemoveTopItemAt(row_X, col_Y);
            if (pickup == null)
                return false;

            return Inventory.AddItem(pickup);
        }

        public bool DropItem(int inventoryIndex, Room currentRoom)
        {
            if (currentRoom == null)
                return false;

            Item? itemDrop = Inventory.LayOutOfInventoryAt(inventoryIndex);
            if (itemDrop == null)
                return false;

            (int row_X, int col_Y) = currentRoom.PlayerPosition;
            currentRoom.ReceiveDropItem(row_X, col_Y, itemDrop);

            return true;
        }

        // TODO: implementation after implemented Weapons, Decorators, etc.
        public bool Equip(int inventoryIndex, int handNumber)
        {
            Item? itemToEquip = Inventory.LayOutOfInventoryAt(inventoryIndex);
            if (itemToEquip == null)
            {
                Console.WriteLine("Nice try. Look into your inventory one more time. Please...");
                return false;
            }

            if (!itemToEquip.CanEquip)
            {
                Console.WriteLine("This is unequable");
                Inventory.AddItem(itemToEquip);
                return false;
            }

            if (itemToEquip.TwoHanded)
            {
                if (Hands[0] != null || Hands[1] != null)
                {
                    Console.WriteLine("It is so proud that it cannot be used with other weapons.");
                    Inventory.AddItem(itemToEquip);
                    return false;
                }
                else
                {
                    Hands[0] = itemToEquip;
                    Hands[1] = itemToEquip;
                }
                
            }
            else
            {
                if (Hands[0] != null && Hands[1] != null)
                {
                    Console.WriteLine("Sometimes third arm can be a really good mutation...");
                    Inventory.AddItem(itemToEquip);
                    return false;
                }
                else if (Hands[handNumber] != null)
                {
                    Console.WriteLine("Maybe, try another hand...");
                    Inventory.AddItem(itemToEquip);
                    return false;
                }                    
                else
                    Hands[handNumber] = itemToEquip;
            }

            // Message about succes equipment.
            // Potentially call of decorators to change charateristics of the player.
            itemToEquip.Equip(this);

            return true;
        }
        public bool Unequip(int handNumber, Room currentRoom)
        {
            if (Hands[handNumber] == null)
            {
                Console.WriteLine("This hand is already free.");
                return false;
            }

            Item itemToUnequip = Hands[handNumber]!;

            if (Hands[handNumber]!.TwoHanded)
            {
                Hands[0] = null;
                Hands[1] = null;
            }
            else
                Hands[handNumber] = null;

            itemToUnequip.Unequip(this);
            HandleUnequipItem(itemToUnequip, currentRoom);

            // Exapmle of output order:
            // You've unequipped ...
            // Property changed by ...
            // (Optional warning) Your invemtory full ..., item has been dropped ...

            return true;
        }

        private void HandleUnequipItem(Item itemToUnequip, Room currentRoom)
        {
            if (Inventory.Items.Count == Inventory.Capacity)
            {
                Console.WriteLine("Your inventory is full. The item has been dropped on the floor.");

                (int row_X, int col_Y) = currentRoom.PlayerPosition;
                currentRoom.ReceiveDropItem(row_X, col_Y, itemToUnequip);
            }
            else
                Inventory.AddItem(itemToUnequip);
        }
    }
}
