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
            Render.Instance.RenderMonsterPanel(_player, _room);

            /* ---------- PLAYER TURN ---------- */
            AttackType attackChoice = AskAttackType();
            IPlayerAttackVisitor atkVisitor = attackChoice switch
            {
                AttackType.Stealth => new StealthAttackVisitor(_player),
                AttackType.Magic => new MagicAttackVisitor(_player),
                _ => new NormalAttackVisitor(_player)
            };

            if (_player.PrimaryWeaponOrNull() is Weapon weapon)
                weapon.Accept(atkVisitor);

            int dmgToEnemy = atkVisitor.Damage;
            _enemy.TakeDamage(dmgToEnemy);

            Render.Instance.StartNewActionMessage();
            Render.Instance.AddActionLine($"You deal {dmgToEnemy} damage to {_enemy.Name} (HP {_enemy.Health}).");

            /* ---------- MONSTER DEFEATED? ---------- */
            if (_enemy.Health <= 0)
            {
                Render.Instance.AddActionLine($"{_enemy.Name} was defeated.");
                EndRound();
                return;
            }

            /* ---------- ENEMY TURN ---------- */
            var enemyAttack = new EnemyAttackVisitor(_enemy.AttackPower); // normal attack
            enemyAttack.VisitPlayer(_player);

            Render.Instance.AddActionLine($"{_enemy.Name} hits you for {enemyAttack.DamageDealt} damage (HP {_player.Health}).");
            Render.Instance.FinalizeActionMessage();

            /* ---------- PLAYER DEAD ---------- */
            if (_player.Health <= 0)
            {
                Render.Instance.StartNewActionMessage();
                Render.Instance.AddActionLine("You have died with all your braveness…");
                Render.Instance.AddActionLine("Game Over!");
                Render.Instance.FinalizeActionMessage();
                PauseForPlayer();
                Environment.Exit(0);
            }

            EndRound();
        }

        private static AttackType AskAttackType()
        {
            Render.Instance.StartNewActionMessage();
            Render.Instance.AddActionLine("Choose attack – N:Normal, S:Stealth, M:Magic");
            Render.Instance.FinalizeActionMessage();
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
            Render.Instance.FinalizeActionMessage();
            Render.Instance.RenderSidePanel(_player, _room);
            Render.Instance.RenderMonsterPanel(_player, _room);
            PauseForPlayer();
        }

        private static void PauseForPlayer() => Console.ReadKey(true); // any key
    }
}
