using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    public sealed class CombatResolvedEvent : BaseEvent
    {
        public int PlayerId { get; set; }
        public int EnemyDamageTaken { get; set; }
        public bool EnemyDefeated { get; set; }
        public int PlayerDamageTaken { get; set; }
        public bool PlayerDefeated { get; set; }
        public string EnemyName { get; set; } = "";
        public string AttackType { get; set; } = "";
    }
}
