using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;
using Rogue.Models.Interfaces;
using Rogue.Models.Combat.Visitors;
using Rogue.Core.Combat;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Rogue.Models.EnemyBehaviour
{
    // Aggressive behavior: moves toward the player if within 3x3 vicinity, attacks if adjacent.
    public class AggressiveBehaviour : IEnemyBehavior
    {
        public void ExecuteBehavior(Enemy enemy, int enemyRow, int enemyCol, Room room, IEnumerable<Player> players, GameUpdateDTO update)
        {
            // Determine player position (assume single player)
            var player = players.First();
            int curPlRow = player.Position.Row;
            int curPlCol = player.Position.Col;

            // Trigger provocation only when player first enters 3×3 area
            if (!enemy.IsProvoked)
            {
                if (Math.Abs(curPlRow - enemyRow) > 1 || Math.Abs(curPlCol - enemyCol) > 1)
                {
                    // Player not yet within 3×3 area
                    return;
                }
                // Player just entered 3×3 area; start stalking
                enemy.IsProvoked = true;
            }
            // Now that provoked, find nearest player anywhere
            Player? targetPlayer = null;
            int targetDistance = int.MaxValue;
            int playerRow = 0, playerCol = 0;
            foreach (Player p in players)
            {
                var (r, c) = p.Position;
                int dist = Math.Abs(r - enemyRow) + Math.Abs(c - enemyCol);
                if (dist < targetDistance)
                {
                    targetDistance = dist;
                    targetPlayer = p;
                    playerRow = r;
                    playerCol = c;
                }
            }
            if (targetPlayer == null)
                return;



            if (targetDistance == 1)
            {
                // Player is adjacent (within 1 tile): perform attack
                EnemyAttackVisitor attack = new EnemyAttackVisitor(enemy.AttackPower);
                attack.VisitPlayer(targetPlayer);
                int damageDealt = attack.DamageDealt;
                // Record combat event (enemy attack) and update player's health status
                update.Events.Add(new CombatResolvedEvent
                {
                    PlayerId = targetPlayer.Id,
                    EnemyName = enemy.Name,
                    AttackType = AttackType.Normal.ToString(),
                    EnemyDamageTaken = 0,
                    EnemyDefeated = false,
                    PlayerDamageTaken = damageDealt,
                    PlayerDefeated = targetPlayer.Health <= 0
                });
                update.Events.Add(new PlayerStatusDto  // update player HP and stats on UI
                {
                    Id = targetPlayer.Id,
                    Health = targetPlayer.Health,
                    Strength = targetPlayer.Strength,
                    Dexterity = targetPlayer.Dexterity,
                    Luck = targetPlayer.Luck,
                    Wisdom = targetPlayer.Wisdom,
                    InventoryCount = targetPlayer.Inventory.Items.Count,
                    Equipped = new List<string> {
                        targetPlayer.Hands[0]?.GetDisplayName() ?? "",
                        targetPlayer.Hands[1]?.GetDisplayName() ?? ""
                    }
                });
            }
            else
            {
                // Player is in range but not adjacent: move one step closer.
                int dRow = 0, dCol = 0;
                if (playerRow < enemyRow) dRow = -1;
                else if (playerRow > enemyRow) dRow = 1;
                if (playerCol < enemyCol) dCol = -1;
                else if (playerCol > enemyCol) dCol = 1;
                // If diagonal, prioritize the larger distance axis
                if (dRow != 0 && dCol != 0)
                {
                    if (Math.Abs(playerRow - enemyRow) >= Math.Abs(playerCol - enemyCol))
                        dCol = 0;
                    else
                        dRow = 0;
                }
                int newR = enemyRow + dRow, newC = enemyCol + dCol;
                if (newR < 0 || newR >= room.Rows || newC < 0 || newC >= room.Columns) return;
                Cell dest = room.Grid[newR, newC];
                // Only move if target cell is free (no wall, no player, no other enemy)
                if (dest.IsWall || dest.PlayerOccupant != null || dest.Enemy != null) return;
                // Update grid: move enemy from old cell to new cell
                room.Grid[enemyRow, enemyCol].Enemy = null;
                dest.Enemy = enemy;
                // Prepare cell updates for rendering
                var (oldSym, oldColor) = room.Grid[enemyRow, enemyCol].GetDisplayCell();
                var (newSym, newColor) = room.Grid[newR, newC].GetDisplayCell();
                update.ChangedCells.Add(new CellUpdateDTO
                {
                    Row = enemyRow,
                    Col = enemyCol,
                    Symbol = oldSym,
                    Color = oldColor,
                    IsWall = room.Grid[enemyRow, enemyCol].IsWall,
                    NowHasPlayer = false,
                    PlayerId = null,
                    NowHasEnemy = false,
                    EnemyId = null
                });
                update.ChangedCells.Add(new CellUpdateDTO
                {
                    Row = newR,
                    Col = newC,
                    Symbol = newSym,
                    Color = newColor,
                    IsWall = room.Grid[newR, newC].IsWall,
                    NowHasPlayer = false,
                    PlayerId = null,
                    NowHasEnemy = true,
                    EnemyId = enemy.GetHashCode()
                });
                // Update enemy's stored position
                enemy.Position = (newR, newC);
            }
        }
    }
}
