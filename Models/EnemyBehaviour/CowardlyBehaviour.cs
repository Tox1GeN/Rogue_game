using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;
using Rogue.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace Rogue.Models.EnemyBehaviour
{
    public class CowardlyBehaviour : IEnemyBehavior
    {
        public void ExecuteBehavior(Enemy enemy, int enemyRow, int enemyCol, Room room, IEnumerable<Player> players, GameUpdateDTO update)
        {
            // Only act when player enters 3×3 area (one-step radius)
            var player = players.First();
            int pr = player.Position.Row;
            int pc = player.Position.Col;
            if (Math.Abs(pr - enemyRow) > 1 || Math.Abs(pc - enemyCol) > 1)
            {
                // Player not in 3×3 → do nothing
                return;
            }
            // Player in 3×3 → flee one step away
            int dr = 0, dc = 0;
            if (pr < enemyRow) dr = 1;
            else if (pr > enemyRow) dr = -1;
            if (pc < enemyCol) dc = 1;
            else if (pc > enemyCol) dc = -1;
            // If diagonal, prioritize axis with larger distance
            if (dr != 0 && dc != 0)
            {
                if (Math.Abs(pr - enemyRow) >= Math.Abs(pc - enemyCol))
                    dc = 0;
                else
                    dr = 0;
            }
            int newR = enemyRow + dr, newC = enemyCol + dc;
            if (newR < 0 || newR >= room.Rows || newC < 0 || newC >= room.Columns)
                return;
            Cell destCell = room.Grid[newR, newC];
            if (destCell.IsWall || destCell.PlayerOccupant != null || destCell.Enemy != null)
                return;
            room.Grid[enemyRow, enemyCol].Enemy = null;
            destCell.Enemy = enemy;
            var (oldSym, oldColor) = room.Grid[enemyRow, enemyCol].GetDisplayCell();
            var (newSym, newColor) = destCell.GetDisplayCell();
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
                IsWall = destCell.IsWall,
                NowHasPlayer = false,
                PlayerId = null,
                NowHasEnemy = true,
                EnemyId = enemy.GetHashCode()
            });
            enemy.Position = (newR, newC);

        }
    }
}
