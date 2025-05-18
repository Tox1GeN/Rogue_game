using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Combat
{
    // Immutable summary used by the server to describe one player–vs-enemy exchange.</summary>
    public sealed class CombatResult
    {
        public int PlayerId          { get; init; }   // the attacker
        public string PlayerNickname { get; init; }
        public int EnemyDamageTaken  { get; init; }
        public bool EnemyDefeated    { get; init; }
        public int PlayerDamageTaken { get; init; }   // retaliation
        public bool PlayerDefeated   { get; init; }
    }
}
