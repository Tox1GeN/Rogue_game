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
    public sealed class EnemyAttackVisitor
    {
        private readonly int _power;
        private readonly AttackType _type;
        public int DamageDealt { get; private set; }
        public EnemyAttackVisitor(int power, AttackType type = AttackType.Normal)
        { _power = power; _type = type; }
        public void VisitPlayer(Player player)
        {
            IDefenseVisitor dv = _type switch
            {
                AttackType.Stealth => new StealthDefenseVisitor(),
                AttackType.Magic => new MagicDefenseVisitor(),
                _ => new NormalDefenseVisitor(),
            };
            player.Accept(dv);
            DamageDealt = Math.Max(0, _power - dv.TotalDefense);
            player.Health -= DamageDealt;
        }
    }
}
