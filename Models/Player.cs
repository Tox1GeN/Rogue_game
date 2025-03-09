using Rogue.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal class Player
    {
        //Player's characteristics
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Health { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }

        // Player has inventory interface
        public Inventory Inventory { get; set; }

        // Hands Logic
        public Item? LeftHand { get; set; }
        public Item? RightHand { get; set; }

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
        }

        // Player Actions
        public void Move (/*future parameteres*/)
        {
            // TODO: Move implementation
        }

        public bool PickupItem(Room currentRoom)
        {
            if ( currentRoom == null)
                return false;

            (int row_X, int col_Y) = currentRoom.PlayerPosition;

            Item? pickup = currentRoom.RemoveTopItemAt(row_X, col_Y);
            if ( pickup == null )
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
        public bool Equip(int inventoryIndex) { return false; }
        public bool Unequip(bool whichHand) { return false; }
    }
}
