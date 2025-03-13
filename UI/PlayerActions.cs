using Rogue.Core;
using Rogue.Models;
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
            Render.StartNewActionMessage();
            if (player.PickupItem(currentRoom))
                Render.RenderSidePanel(player, currentRoom);
        }

        public static void PressDrop(Player player, Room currentRoom)
        {
            Render.StartNewActionMessage();
            if (player.Inventory.Items.Count == 0)
            {
                Render.AddActionLine("You don't have anything anyway");
                Render.FinalizeActionMessage();
                return;
            }
            Render.AddActionLine("Enter item's id: ");

            Render.FinalizeActionMessage();

            string? indexInput = Console.ReadLine();

            Render.StartNewActionMessage();

            if (int.TryParse(indexInput, out int invIndex))
            {
                // Get the item from the inventory
                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    Render.AddActionLine("Nice try. Look into your inventory one more time. Please...");
                    Render.FinalizeActionMessage();
                    return;
                }
                else
                {
                    if (player.DropItem(invIndex, currentRoom))
                        Render.AddActionLine($"You dropped: {item.GetDisplayName()}");
                }
            }
            else
            {
                Render.AddActionLine("Invalid input. Must be the number.");
            }

            Render.FinalizeActionMessage();
            Render.RenderSidePanel(player, currentRoom);
        }

        public static void PressEquip(Player player, Room currentRoom)
        {
            Render.StartNewActionMessage();
            Render.AddActionLine("Enter equipment's id:");
            Render.FinalizeActionMessage();

            string? indexInput = Console.ReadLine();
            Render.StartNewActionMessage();

            if (int.TryParse(indexInput, out int invIndex))
            {
                // Get the item from the inventory
                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    Render.AddActionLine("Nice try. Look into your inventory one more time. Please...");
                    Render.FinalizeActionMessage();
                    return;
                }
                else if (!item.CanEquip)
                {
                    Render.AddActionLine("This is unequippable");
                    Render.FinalizeActionMessage();
                    return;
                }
                else
                {
                    if (!item.TwoHanded)
                    {
                        Render.AddActionLine("Choose hand (0 - L, 1 - R): ");
                        Render.FinalizeActionMessage();
                        string? handInput = Console.ReadLine();

                        Render.StartNewActionMessage();
                        if (int.TryParse(handInput, out int handNumber) && (handNumber == 0 || handNumber == 1))
                        {
                            if (player.Equip(item, handNumber))
                                player.Inventory.RemoveItemAt(invIndex);
                        }
                        else
                        {
                            Render.AddActionLine("Invalid hand selection. Must be the number 0 or 1");
                        }
                    }
                    else
                    {
                        // For two-handed items, call equip without additional prompt.
                        player.Equip(item, 0);
                        player.Inventory.RemoveItemAt(invIndex);
                    }
                }
            }
            else
            {
                Render.AddActionLine("Invalid input. Must be the number.");
            }

            Render.FinalizeActionMessage();
            Render.RenderSidePanel(player, currentRoom);
        }

        public static void PressUnequip(Player player, Room currentRoom)
        {
            Render.StartNewActionMessage();
            if (player.Hands[0] == null && player.Hands[1] == null)
            {
                Render.AddActionLine("You have nothing in you hands");
                Render.FinalizeActionMessage();
                return;
            }
            else if (player.Hands[0] != null && player.Hands[0]!.TwoHanded)
                player.Unequip(0, currentRoom);
            else
            {
                Render.AddActionLine("Choose hand to free (0 - L, 1 - R)");
                Render.FinalizeActionMessage();

                string? hand = Console.ReadLine();
                Render.StartNewActionMessage();

                if (int.TryParse(hand, out int handNumber))
                {
                    player.Unequip(handNumber, currentRoom);
                }
                else
                {
                    Render.AddActionLine("Invalid input. Must be the number 0 or 1.");
                }
            }

            Render.FinalizeActionMessage();
            Render.RenderSidePanel(player, currentRoom);
        }
    }
}
