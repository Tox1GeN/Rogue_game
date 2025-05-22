using Rogue.Network.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Core;
using System.Text.Json;
using System.Net;
using Rogue.Network.Dto.Events;
using Rogue.Models.UsableItems.Potions;

namespace Rogue.Network.Server
{
    public sealed partial class Server
    {
        private void HandleAction(ActionRequestDto actReqDto, Player player)
        {
            lock (_stateLock)
            {
                // Reject if not player's turn
                if (_gameState.Players[_gameState.CurrentPlayerTurnIndex].Id != player.Id)
                    return;

                switch (actReqDto.Action)
                {
                    case "Move":
                        if (DoMove(actReqDto.Direction!, player))      // returns TRUE if step really happened
                        {
                            _gameState.MovesRemaining--;
                            if (_gameState.MovesRemaining <= 0) FinishTurn();
                        }
                        break;
                    case "Pickup":    DoPickup(player); break;
                    case "Drop":      DoDrop(player, actReqDto.InventoryIndex ?? -1); break;
                    case "UsePotion": DoUsePotion(player, actReqDto.InventoryIndex ?? -1); break;
                    case "Equip":     DoEquip(player, actReqDto.InventoryIndex ?? -1, actReqDto.HandNumber ?? -1); break;
                    case "Unequip":   DoUnequip(player, actReqDto.HandNumber ?? -1); break;
                    case "EndTurn":   FinishTurn(); return;
                }
            }
        }

        private bool DoMove(string dir, Player player)
        {
            (int dr, int dc) = dir switch
            {
                "Up" => (-1, 0),
                "Down" => (1, 0),
                "Left" => (0, -1),
                "Right" => (0, 1),
                _ => (0, 0)
            };

            if (dr == 0 && dc == 0) return false; // no move requested



            int nr = player.Position.Row + dr;
            int nc = player.Position.Col + dc;

            Cell destCell = _gameState.Dungeon.Grid[nr, nc];
            if (!Inside(nr, nc) || destCell.IsWall || destCell.PlayerOccupant != null)
                return false;

            if (destCell.Enemy != null)
            {
                ScheduleCombat(player, destCell.Enemy, nr, nc);
                return false;
            }

            /* ------------------------------------------------
             * TODO: Refactor Move in PLayer class,
             * so no Render will be called
             -------------------------------------------------*/

            // int oldR = pl.Position.Row, oldC = pl.Position.Col;

            // pl.Move(dr, dc, _gameState.Dungeon);
            //// (still contains Render calls – harmless on headless server)

            // if (pl.Position.Row == oldR && pl.Position.Col == oldC)
            //    return false;

            var from = player.Position;
            _gameState.Dungeon.Grid[from.Row, from.Col].PlayerOccupant = null;
            destCell.PlayerOccupant = player;
            player.Position = (nr, nc);



            var update = new GameUpdateDTO();
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, from.Row, from.Col));
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, nr, nc));
            update.Events.Add(new PlayerMovedEvent
            {
                PlayerId = player.Id,
                FromRow = from.Row,
                FromCol = from.Col,
                ToRow = nr,
                ToCol = nc
            });


            Broadcast(JsonSerializer.Serialize(update));


            return true;
        }

        private void ScheduleCombat(Player player, Enemy enemy, int destRow, int destCol)
        {
            var pc = new PendingCombat
            {
                Attacker = player,
                Target = enemy,
                DestRow = destRow,
                DestCol = destCol
            };
            _pending[player.Id] = pc;

            var req = new AttackChoiceRequestDto
            {
                PlayerId = player.Id,
                EnemyId = enemy.GetHashCode(),
                EnemyName = enemy.Name
            };
            SendToClient(player, JsonSerializer.Serialize(req));
        }

        private void DoPickup(Player player)
        {
            bool wasPickedup = player.PickupItem(_gameState.Dungeon);
            if (!wasPickedup) return;

            var update = new GameUpdateDTO();
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, player.Position.Row, player.Position.Col));
            update.Events.Add(new ItemPickedUpEvent
            {
                PlayerId = player.Id,
                ItemName = player.Inventory.Items.Last().GetDisplayName()
            });
            update.Events.Add(MakeInvStat(player));

            var (r, c) = player.Position;
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, r, c));

            Broadcast(JsonSerializer.Serialize(update));
        }
        private void DoDrop(Player player, int invIndex)
        {
            if (invIndex < 0 || invIndex >= player.Inventory.Items.Count) return;

            Item? drop = player.Inventory.ItemAt(invIndex);
            if (drop == null) return;

            bool wasDropped = player.DropItem(invIndex, _gameState.Dungeon); // original method
            if (!wasDropped) return;

            var update = new GameUpdateDTO();
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, player.Position.Row, player.Position.Col));
            update.Events.Add(new ItemDroppedEvent
            {
                PlayerId = player.Id,
                ItemName = drop.GetDisplayName()
            });
            update.Events.Add(MakeInvStat(player));

            var (r, c) = player.Position;
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon, r, c));

            Broadcast(JsonSerializer.Serialize(update));
        }

        private void DoUsePotion(Player player, int invIndex)
        {
            if (invIndex < 0 || invIndex >= player.Inventory.Items.Count) return;

            Item potion = player.Inventory.Items[invIndex];
            if (!potion.CanUse) return;

            player.UseItem(potion, invIndex);

            var update = new GameUpdateDTO();
            update.Events.Add(new PotionUsedEvent
            {
                PlayerId = player.Id,
                ItemName = potion.GetDisplayName()
            });
            update.Events.Add(MakeInvStat(player));

            Broadcast(JsonSerializer.Serialize(update));
        }

        private void DoEquip (Player player, int invIndex, int handNum)
        {
            if (invIndex < 0 || invIndex >= player.Inventory.Items.Count) return;

            Item item = player.Inventory.Items[invIndex];
            if (!item.CanEquip) return;

            bool wasEquipped = player.Equip(item, handNum);
            if (!wasEquipped) return;

            player.Inventory.RemoveItemAt(invIndex);

            var update = new GameUpdateDTO();
            update.Events.Add(new ItemEquippedEvent
            {
                PlayerId = player.Id,
                ItemName = item.GetDisplayName(),
                HandNumber = handNum
            });

            update.Events.Add(MakeInvStat(player));

            Broadcast(JsonSerializer.Serialize(update));
        }

        private void DoUnequip(Player player, int handNum)
        {
            if (handNum < 0 || handNum > 1) return;

            Item? held = player.Hands[handNum];
            if (held == null) return;

            bool wasUnequipped = player.Unequip(handNum, _gameState.Dungeon);
            if (!wasUnequipped) return;

            var update = new GameUpdateDTO();
            update.Events.Add(new ItemUnequippedEvent
            {
                PlayerId = player.Id,
                ItemName = held.GetDisplayName()
            });
            update.Events.Add(MakeInvStat(player));

            // In case of dropped item on the floor
            update.ChangedCells.Add(MakeCell(_gameState.Dungeon,
                                          player.Position.Row, player.Position.Col));

            Broadcast(JsonSerializer.Serialize(update));
        }
    }
}
