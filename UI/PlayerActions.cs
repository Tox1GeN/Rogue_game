using Rogue.Core;
using Rogue.Models;
using Rogue.Models.Interfaces;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI
{
    public class PlayerActions
    {
        public static void PressPickUp(Player player, Room currentRoom)
        {
            MessageBuffer.Begin();
            if (player.PickupItem(currentRoom))
                RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
        }

        public static void PressDrop(Player player, Room currentRoom)
        {
            if (player.Inventory.Items.Count == 0)
            {
                RenderDispatcher.Raise(new RenderActionMessageEvent(new[]
                {
                    "You don't have anything anyway"
                }));
                return;
            }

            RenderDispatcher.Raise(new RequestTextInputEvent("Enter item's id: ", input =>
            {
                MessageBuffer.Begin();

                if (int.TryParse(input, out int invIndex))
                {
                    // Get the item from the inventory
                    Item? item = player.Inventory.ItemAt(invIndex);
                    if (item == null)
                    {
                        MessageBuffer.Add("Nice try. Look into your inventory one more time. Please...");
                    }
                    else
                    {
                        if (player.DropItem(invIndex, currentRoom))
                            MessageBuffer.Add($"You dropped: {item.GetDisplayName()}");
                        else
                            MessageBuffer.Add("Couldn't drop the item for some reason.");
                    }
                }
                else
                {
                    MessageBuffer.Add("Invalid input. Must be the number.");
                }
                
                MessageBuffer.Commit();
                RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
            }));
        }

        public static void PressEquip(Player player, Room currentRoom)
        {
            // Ask for item index
            RenderDispatcher.Raise(new RequestTextInputEvent("Enter equipment's id:", input =>
            {
                if (!int.TryParse(input, out int invIndex))
                {
                    RenderDispatcher.Raise(new RenderActionMessageEvent(new[] { "Invalid input. Must be a number." }));
                    return;
                }

                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    RenderDispatcher.Raise(new RenderActionMessageEvent(new[] { "Nice try. Look into your inventory one more time. Please..." }));
                    return;
                }

                if (!item.CanEquip)
                {
                    RenderDispatcher.Raise(new RenderActionMessageEvent(new[] { "This is unequippable" }));
                    return;
                }

                // Two-handed item — equip immediately
                if (item.TwoHanded)
                {
                    MessageBuffer.Begin();
                    if (player.Equip(item, 0))
                    {
                        player.Inventory.RemoveItemAt(invIndex);
                        MessageBuffer.Add($"You equipped {item.GetDisplayName()} in both hands.");
                    }
                    else
                    {
                        MessageBuffer.Add("Failed to equip item.");
                    }
                    MessageBuffer.Commit();
                    RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
                    return;
                }

                // Ask for hand input
                RenderDispatcher.Raise(new RequestTextInputEvent("Choose hand (0 - L, 1 - R):", handInput =>
                {
                    MessageBuffer.Begin();

                    if (int.TryParse(handInput, out int handNumber) && (handNumber == 0 || handNumber == 1))
                    {
                        if (player.Equip(item, handNumber))
                        {
                            player.Inventory.RemoveItemAt(invIndex);
                            MessageBuffer.Add($"You equipped {item.GetDisplayName()} in {(handNumber == 0 ? "left" : "right")} hand.");
                        }
                        else
                        {
                            MessageBuffer.Add("Failed to equip item.");
                        }
                    }
                    else
                    {
                        MessageBuffer.Add("Invalid hand selection. Must be 0 or 1.");
                    }

                    MessageBuffer.Commit();
                    RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
                }));
            }));
        }


        public static void PressUnequip(Player player, Room currentRoom)
        {
            // If both hands are empty
            if (player.Hands[0] == null && player.Hands[1] == null)
            {
                RenderDispatcher.Raise(new RenderActionMessageEvent(new[] { "You have nothing in your hands." }));
                return;
            }

            // If two-handed item in left
            if (player.Hands[0] != null && player.Hands[0]!.TwoHanded)
            {
                player.Unequip(0, currentRoom);
                RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
                return;
            }

            // Ask user which hand
            RenderDispatcher.Raise(new RequestTextInputEvent("Choose hand to free (0 - L, 1 - R):", handInput =>
            {
                MessageBuffer.Begin();

                if (int.TryParse(handInput, out int handNumber) && (handNumber == 0 || handNumber == 1))
                {
                    player.Unequip(handNumber, currentRoom);
                }
                else
                {
                    MessageBuffer.Add("Invalid input. Must be 0 or 1.");
                }

                MessageBuffer.Commit();
                RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
            }));
        }


        public static void PressUse(Player player, Room currentRoom)
        {
            if (player.Inventory.Items.Count == 0)
            {
                RenderDispatcher.Raise(new RenderActionMessageEvent(new[] { "You don't have anything anyway." }));
                return;
            }

            RenderDispatcher.Raise(new RequestTextInputEvent("Enter item's id:", input =>
            {
                MessageBuffer.Begin();

                if (int.TryParse(input, out int invIndex))
                {
                    Item? item = player.Inventory.ItemAt(invIndex);
                    if (item == null)
                    {
                        MessageBuffer.Add("Nice try. Look into your inventory one more time. Please...");
                    }
                    else if (!item.CanUse)
                    {
                        MessageBuffer.Add("This item cannot be used.");
                    }
                    else
                    {
                        player.UseItem(item, invIndex);
                    }
                }
                else
                {
                    MessageBuffer.Add("Invalid input. Must be a number.");
                }

                MessageBuffer.Commit();
                RenderDispatcher.Raise(new RenderSidePanelEvent(player, currentRoom));
            }));
        }

    }
}
