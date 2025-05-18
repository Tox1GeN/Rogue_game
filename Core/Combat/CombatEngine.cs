using Rogue.Models;
using Rogue.Models.Combat.Visitors;
using Rogue.Models.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Combat
{
    /// <param name="attackType">
    ///   Normal / Magic / Stealth — the server may choose a default or the client
    ///   can send the desired type in its ActionRequestDTO.
    /// </param>
    public static class CombatEngine
    {

        // TODO: return for refactoring after DTOs declaring and implementation
        public static CombatResult Resolve(Player player, Enemy enemy, AttackType attackType = AttackType.Normal)
        {
            IPlayerAttackVisitor atkVisitor = attackType switch
            {
                AttackType.Stealth => new StealthAttackVisitor(player),
                AttackType.Magic   => new MagicAttackVisitor(player),
                _                  => new NormalAttackVisitor(player)
            };


            var wpn = player.PrimaryWeaponOrNull();
            if (wpn != null)
                wpn.Accept(atkVisitor);
            // { set; } for Damage ???
            // else
            //    atkVisitor.Damage = player.Strength;

            int dmgToEnemy = atkVisitor.Damage;
            enemy.TakeDamage(dmgToEnemy);

            bool enemyDead = enemy.Health <= 0;
            int dmgToPlayer = 0;
            bool playerDead = false;

            if (!enemyDead)
            {
                var enemyAtk = new EnemyAttackVisitor(enemy.AttackPower);
                enemyAtk.VisitPlayer(player);
                dmgToPlayer = enemyAtk.DamageDealt;
                playerDead = player.Health <= 0;
            }

            return new CombatResult
            {
                PlayerId = player.Id,
                PlayerNickname = player.Nickname,
                EnemyDamageTaken = dmgToEnemy,
                EnemyDefeated = enemyDead,
                PlayerDamageTaken = dmgToPlayer,
                PlayerDefeated = playerDead
            };
        }
    }
}
