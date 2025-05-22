using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Models.Combat.Visitors;
using Rogue.UI;
using Rogue.Models.Weapons;


namespace Rogue.Core.Combat
{
    public sealed class CombatSession
    {
        private readonly Player _player;
        private readonly Enemy _enemy;
        private readonly Room _room;

        public CombatSession(Player player, Enemy enemy, Room room)
        {
            _player = player;
            _enemy = enemy;
            _room = room;
        }

        public void Start()
        {
            // Always show enemy panel before the round
            RenderDispatcher.Raise(new RenderMonsterPanelEvent(_player, _room));

            /* ---------- PLAYER TURN ---------- */
            AttackType attackChoice = AskAttackType();
            IPlayerAttackVisitor atkVisitor = attackChoice switch
            {
                AttackType.Stealth => new StealthAttackVisitor(_player),
                AttackType.Magic => new MagicAttackVisitor(_player),
                _ => new NormalAttackVisitor(_player)
            };

            var weapon = _player.PrimaryWeaponOrNull();
            if (weapon != null)
                weapon.Accept(atkVisitor);


            int dmgToEnemy = atkVisitor.Damage;
            _enemy.TakeDamage(dmgToEnemy);

            MessageBuffer.Begin();
            MessageBuffer.Add($"You deal {dmgToEnemy} damage to {_enemy.Name} (HP {_enemy.Health}).");
            
            /* ---------- MONSTER DEFEATED? ---------- */
            if (_enemy.Health <= 0)
            {
                MessageBuffer.Add($"{_enemy.Name} was defeated.");
                EndRound();
                return;
            }

            /* ---------- ENEMY TURN ---------- */
            var enemyAttack = new EnemyAttackVisitor(_enemy.AttackPower); // normal attack
            enemyAttack.VisitPlayer(_player);

            MessageBuffer.Add($"{_enemy.Name} hits you for {enemyAttack.DamageDealt} damage (HP {_player.Health}).");
            MessageBuffer.Commit();

            /* ---------- PLAYER DEAD ---------- */
            if (_player.Health <= 0)
            {
                MessageBuffer.Begin();
                MessageBuffer.Add("You have died with all your braveness…");
                MessageBuffer.Add("Game Over!");
                MessageBuffer.Commit();
                PauseForPlayer();
                Environment.Exit(0);
            }

            EndRound();
        }

        private static AttackType AskAttackType()
        {
            MessageBuffer.Begin();
            MessageBuffer.Add("Choose attack – N:Normal, S:Stealth, M:Magic");
            MessageBuffer.Commit();
            ConsoleKey ck = Console.ReadKey(true).Key;
            return ck switch
            {
                ConsoleKey.S => AttackType.Stealth,
                ConsoleKey.M => AttackType.Magic,
                _ => AttackType.Normal,
            };
        }

        private void EndRound()
        {
            MessageBuffer.Commit();
            RenderDispatcher.Raise(new RenderSidePanelEvent(_player, _room));
            RenderDispatcher.Raise(new RenderMonsterPanelEvent(_player, _room));

            PauseForPlayer();
        }

        private static void PauseForPlayer() => Console.ReadKey(true); // any key
    }
}
