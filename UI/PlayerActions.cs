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
            Render.Instance.StartNewActionMessage();
            if (player.PickupItem(currentRoom))
                Render.Instance.RenderSidePanel(player, currentRoom);
        }

        public static void PressDrop(Player player, Room currentRoom)
        {
            Render.Instance.StartNewActionMessage();
            if (player.Inventory.Items.Count == 0)
            {
                Render.Instance.AddActionLine("You don't have anything anyway");
                Render.Instance.FinalizeActionMessage();
                return;
            }
            Render.Instance.AddActionLine("Enter item's id: ");

            Render.Instance.FinalizeActionMessage();

            string? indexInput = Console.ReadLine();

            Render.Instance.StartNewActionMessage();

            if (int.TryParse(indexInput, out int invIndex))
            {
                // Get the item from the inventory
                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    Render.Instance.AddActionLine("Nice try. Look into your inventory one more time. Please...");
                    Render.Instance.FinalizeActionMessage();
                    return;
                }
                else
                {
                    if (player.DropItem(invIndex, currentRoom))
                        Render.Instance.AddActionLine($"You dropped: {item.GetDisplayName()}");
                }
            }
            else
            {
                Render.Instance.AddActionLine("Invalid input. Must be the number.");
            }

            Render.Instance.FinalizeActionMessage();
            Render.Instance.RenderSidePanel(player, currentRoom);
        }

        public static void PressEquip(Player player, Room currentRoom)
        {
            Render.Instance.StartNewActionMessage();
            Render.Instance.AddActionLine("Enter equipment's id:");
            Render.Instance.FinalizeActionMessage();

            string? indexInput = Console.ReadLine();
            Render.Instance.StartNewActionMessage();

            if (int.TryParse(indexInput, out int invIndex))
            {
                // Get the item from the inventory
                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    Render.Instance.AddActionLine("Nice try. Look into your inventory one more time. Please...");
                    Render.Instance.FinalizeActionMessage();
                    return;
                }
                else if (!item.CanEquip)
                {
                    Render.Instance.AddActionLine("This is unequippable");
                    Render.Instance.FinalizeActionMessage();
                    return;
                }
                else
                {
                    if (!item.TwoHanded)
                    {
                        Render.Instance.AddActionLine("Choose hand (0 - L, 1 - R): ");
                        Render.Instance.FinalizeActionMessage();
                        string? handInput = Console.ReadLine();

                        Render.Instance.StartNewActionMessage();
                        if (int.TryParse(handInput, out int handNumber) && (handNumber == 0 || handNumber == 1))
                        {
                            if (player.Equip(item, handNumber))
                                player.Inventory.RemoveItemAt(invIndex);
                        }
                        else
                        {
                            Render.Instance.AddActionLine("Invalid hand selection. Must be the number 0 or 1");
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
                Render.Instance.AddActionLine("Invalid input. Must be the number.");
            }

            Render.Instance.FinalizeActionMessage();
            Render.Instance.RenderSidePanel(player, currentRoom);
        }

        public static void PressUnequip(Player player, Room currentRoom)
        {
            Render.Instance.StartNewActionMessage();
            if (player.Hands[0] == null && player.Hands[1] == null)
            {
                Render.Instance.AddActionLine("You have nothing in you hands");
                Render.Instance.FinalizeActionMessage();
                return;
            }
            else if (player.Hands[0] != null && player.Hands[0]!.TwoHanded)
                player.Unequip(0, currentRoom);
            else
            {
                Render.Instance.AddActionLine("Choose hand to free (0 - L, 1 - R)");
                Render.Instance.FinalizeActionMessage();

                string? hand = Console.ReadLine();
                Render.Instance.StartNewActionMessage();

                if (int.TryParse(hand, out int handNumber))
                {
                    player.Unequip(handNumber, currentRoom);
                }
                else
                {
                    Render.Instance.AddActionLine("Invalid input. Must be the number 0 or 1.");
                }
            }

            Render.Instance.FinalizeActionMessage();
            Render.Instance.RenderSidePanel(player, currentRoom);
        }

        public static void PressUse(Player player, Room currentRoom)
        {


            Render.Instance.StartNewActionMessage();
            if (player.Inventory.Items.Count == 0)
            {
                Render.Instance.AddActionLine("You don't have anything anyway");
                Render.Instance.FinalizeActionMessage();
                return;
            }
            Render.Instance.AddActionLine("Enter item's id: ");
            Render.Instance.FinalizeActionMessage();

            string? indexInput = Console.ReadLine();

            Render.Instance.StartNewActionMessage();

            if (int.TryParse(indexInput, out int invIndex))
            {
                // Get the item from the inventory
                Item? item = player.Inventory.ItemAt(invIndex);
                if (item == null)
                {
                    Render.Instance.AddActionLine("Nice try. Look into your inventory one more time. Please...");
                    Render.Instance.FinalizeActionMessage();
                    return;
                }
                else if (!item.CanUse)
                {
                    Render.Instance.AddActionLine("This is unusable");
                    Render.Instance.FinalizeActionMessage();
                    return;
                }
                else
                {
                    player.UseItem(item, invIndex);
                }
            }
            else
            {
                Render.Instance.AddActionLine("Invalid input. Must be the number.");
            }

            Render.Instance.FinalizeActionMessage();
            Render.Instance.RenderSidePanel(player, currentRoom);
        }
    }
}
