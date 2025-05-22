using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rogue.Network.Dto
{
    public sealed class AttackChoiceRequestDto
    {
        [JsonPropertyName("type")]
        public string Type => "AttackChoiceRequest";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("enemyId")]
        public int EnemyId { get; init; }

        [JsonPropertyName("enemyName")]
        public string EnemyName { get; init; } = string.Empty;
    }
}
