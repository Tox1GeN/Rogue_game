using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rogue.Network.Dto.Events
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PlayerJoinedEvent), nameof(PlayerJoinedEvent))]
    [JsonDerivedType(typeof(PlayerLeftEvent), nameof(PlayerLeftEvent))]
    [JsonDerivedType(typeof(PlayerMovedEvent), nameof(PlayerMovedEvent))]
    [JsonDerivedType(typeof(ItemPickedUpEvent), nameof(ItemPickedUpEvent))]
    [JsonDerivedType(typeof(ItemDroppedEvent), nameof(ItemDroppedEvent))]
    [JsonDerivedType(typeof(ItemEquippedEvent), nameof(ItemEquippedEvent))]
    [JsonDerivedType(typeof(ItemUnequippedEvent), nameof(ItemUnequippedEvent))]
    [JsonDerivedType(typeof(PotionUsedEvent), nameof(PotionUsedEvent))]
    [JsonDerivedType(typeof(CombatResolvedEvent), nameof(CombatResolvedEvent))]
    [JsonDerivedType(typeof(TurnChangedEvent), nameof(TurnChangedEvent))]
    [JsonDerivedType(typeof(PlayerStatusDto), nameof(PlayerStatusDto))]
    public abstract class BaseEvent
    {
        [JsonIgnore]
        public string EventType => GetType().Name;
    }
}
