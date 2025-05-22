using Rogue.Core;
using Rogue.Core.Combat;
using Rogue.Models;
using Rogue.Models.Combat.Visitors;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rogue.Network.Server
{
    public sealed partial class Server
    {
        private void HandleAttackChoice(AttackChoiceResponseDto dto, Player player)
        {
            if (!_pending.TryRemove(player.Id, out PendingCombat pendingCombat)) return;

            AttackType type = dto.Choice switch
            {
                "Magic" => AttackType.Magic,
                "Stealth" => AttackType.Stealth,
                _ => AttackType.Normal
            };


            // There are possible update for combatResult.EnemyDefeated to true
            CombatResult combatResult = CombatEngine.Resolve(player, pendingCombat.Target, type);

            // building update
            var update = new GameUpdateDTO();
            update.Events.Add(new CombatResolvedEvent
            {
                PlayerId = player.Id,
                EnemyName = pendingCombat.Target.Name,
                AttackType = type.ToString(),
                EnemyDamageTaken = combatResult.EnemyDamageTaken,
                EnemyDefeated = combatResult.EnemyDefeated,
                PlayerDamageTaken = combatResult.PlayerDamageTaken,
                PlayerDefeated = combatResult.PlayerDefeated
            });

            if (combatResult.EnemyDefeated)
            {
                _gameState.Dungeon.Grid[pendingCombat.DestRow, pendingCombat.DestCol].Enemy = null;
                _gameState.Enemies.Remove(pendingCombat.Target);
                update.ChangedCells.Add(MakeCell(_gameState.Dungeon, pendingCombat.DestRow, pendingCombat.DestCol));
            }

            if (combatResult.PlayerDamageTaken > 0)
                update.Events.Add(MakeInvStat(player));

            Broadcast(JsonSerializer.Serialize(update));

            if (_gameState.MovesRemaining <= 0) FinishTurn();
        }        
    }
}
