using Rogue.Core;
using Rogue.UI;
using Rogue.Models.Currency;
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

        // Money talks
        public Coin Coins { get; private set; }
        public Gold GoldValue { get; private set; }

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
            Coins = new Coin(0);
            GoldValue = new Gold(0);
            Hands = new Item?[2]; // index 0: left hand, index 1: right hand
        }

        // Player Actions
        public void Move (int deltaX, int deltaY, Room currentRoom)
        {
            var (row, col) = currentRoom.PlayerPosition;

            int newRow = row + deltaX;
            int newCol = col + deltaY;

            // Out of the boundaries 
            if(newRow < 0 || newRow > 19 || newCol < 0 || newCol > 39)
                return;

            if (currentRoom.Grid[newRow, newCol].IsWall)
                return;

            // Otherwise move the player
            currentRoom.Grid[row, col].IsPlayerHere = false;
            currentRoom.Grid[newRow, newCol].IsPlayerHere = true;

            currentRoom.PlayerPosition = (newRow, newCol);


            Render.RedrawCell(row, col, currentRoom);
            Render.RedrawCell(newRow, newCol, currentRoom);
            Render.RenderSidePanel(this, currentRoom);
        }
        public bool PickupItem(Room currentRoom)
        {
            if (currentRoom == null)
                return false;

            (int row_X, int col_Y) = currentRoom.PlayerPosition;

            Item? pickup = currentRoom.RemoveTopItemAt(row_X, col_Y);
            if (pickup == null)
                return false;


            if (Inventory.AddItem(pickup))
            {
                Render.AddActionLine($"You've picked up the {pickup.GetDisplayName()}");
                Render.FinalizeActionMessage();
                return true;
            }
            else
                return false;
        }
        public bool DropItem(int inventoryIndex, Room currentRoom)
        {
            if (currentRoom == null)
                return false;

            Item? itemDrop = Inventory.ItemAt(inventoryIndex);
            if (itemDrop == null)
                return false;
            else
                Inventory.RemoveItemAt(inventoryIndex);

            (int row_X, int col_Y) = currentRoom.PlayerPosition;
            currentRoom.ReceiveDropItem(row_X, col_Y, itemDrop);

            return true;
        }
        public bool Equip(Item itemToEquip, int handNumber)
        {
            if (itemToEquip.TwoHanded)
            {
                if (Hands[0] != null || Hands[1] != null)
                {
                    Render.AddActionLine("It is so proud that it cannot be used with other weapons.");
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
                    Render.AddActionLine("Sometimes third arm can be a really good mutation...");
                    return false;
                }
                else if (Hands[handNumber] != null)
                {
                    Render.AddActionLine("Maybe, try another hand...");
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
                Render.AddActionLine("This hand is already free.");
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
                Render.AddActionLine("Your inventory is full. The item has been dropped on the floor.");

                (int row_X, int col_Y) = currentRoom.PlayerPosition;
                currentRoom.ReceiveDropItem(row_X, col_Y, itemToUnequip);
            }
            else
            {
                if (Inventory.AddItem(itemToUnequip))
                    Render.AddActionLine("You've hid it in the bag");
            }
        }
    }
}
