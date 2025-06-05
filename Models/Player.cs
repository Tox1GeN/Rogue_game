using Rogue.Core;
using Rogue.UI;
using Rogue.Models.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;
using Rogue.Models.Combat.Visitors;
using System.Drawing;

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

        // For multiplayer
        public int Id { get; set; } = 0;
        public string Nickname { get; set; } = "Player"; // Default name
        public ConsoleColor Color { get; set; } = ConsoleColor.White; // Default color
        public (int Row, int Col) Position { get; set; } = (0, 0); // Player's position in the room

        // Player's buffs or negative effects (observers)
        private List<IEffect> activeEffects = new List<IEffect>();
        public IReadOnlyList<IEffect> GetActiveEffects() => activeEffects.AsReadOnly();

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
            Health = 50;
            Luck = 1;
            Aggression = 0;
            Wisdom = 0;
            Inventory = new Inventory();
            Coins = new Coin(0);
            GoldValue = new Gold(0);
            Hands = new Item?[2]; // index 0: left hand, index 1: right hand
        }

        // Attach and Detach observers

        public void AttachEffect(IEffect effect)
        {
            activeEffects.Add(effect);
        }

        public void DetachEffect(IEffect effect)
        {
            activeEffects.Remove(effect);
        }

        // Visitors helper
        public IWeapon? PrimaryWeaponOrNull()
        {
            foreach (var equipment in Hands)
            {
                var w = equipment?.AsWeapon();
                if (w != null) return w;
            }
            return null;
        }

        public void Accept(IDefenseVisitor visitor)
        {
            visitor.VisitPlayer(this);
            foreach (var equipment in Hands)
                equipment?.Accept(visitor);
        }

        // Notifier of updates

        public void UpdateEffectsPerTurn()
        {
            if (activeEffects.Count == 0)
                return;

            var currentEffects = new List<IEffect>(activeEffects);
            foreach (IEffect effect in currentEffects)
            {
                effect.OnTurnPassed(this);
            }

            activeEffects.RemoveAll(e => e.IsExpired);
        }

        // Player Actions
        public void Move(int deltaX, int deltaY, Room currentRoom)
        {
            var (row, col) = currentRoom.PlayerPosition;

            int newRow = row + deltaX;
            int newCol = col + deltaY;

            // Out of the boundaries 
            if (newRow < 0 || newRow > 19 || newCol < 0 || newCol > 39)
                return;

            if (currentRoom.Grid[newRow, newCol].IsWall)
                return;

            // Otherwise move the player
            currentRoom.Grid[row, col].IsPlayerHere = false;
            currentRoom.Grid[newRow, newCol].IsPlayerHere = true;

            currentRoom.Grid[row, col].PlayerOccupant = null;
            currentRoom.Grid[newRow, newCol].PlayerOccupant = this;

            currentRoom.PlayerPosition = (newRow, newCol);
            Position = (newRow, newCol);


            Render.Instance.RedrawCell(row, col, currentRoom);
            Render.Instance.RedrawCell(newRow, newCol, currentRoom);
            Render.Instance.RenderSidePanel(this, currentRoom);
            Render.Instance.RenderMonsterPanel(this, currentRoom);

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
                MessageBuffer.Add($"You've picked up the {pickup.GetDisplayName()}");
                MessageBuffer.Commit();
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
                    MessageBuffer.Add("It is so proud that it cannot be used with other weapons.");
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
                    MessageBuffer.Add("Sometimes third arm can be a really good mutation...");
                    return false;
                }
                else if (Hands[handNumber] != null)
                {
                    MessageBuffer.Add("Maybe, try another hand...");
                    return false;
                }
                else
                    Hands[handNumber] = itemToEquip;
            }

            itemToEquip.Equip(this);

            return true;
        }
        public bool Unequip(int handNumber, Room currentRoom)
        {
            if (Hands[handNumber] == null)
            {                
                MessageBuffer.Add("This hand is already free.");
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
                MessageBuffer.Add("Your inventory is full. The item has been dropped on the floor.");

                (int row_X, int col_Y) = currentRoom.PlayerPosition;
                currentRoom.ReceiveDropItem(row_X, col_Y, itemToUnequip);
            }
            else
            {
                if (Inventory.AddItem(itemToUnequip))
                    MessageBuffer.Add("You've hid it in the bag");
            }
        }

        public void UseItem(Item item, int invIndex)
        {
            item.Use(this);
            Inventory.RemoveItemAt(invIndex);

            MessageBuffer.Add($"You used: {item.GetDisplayName()}");
        }
    }
}
