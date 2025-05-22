using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rogue.Network.Dto
{
    public sealed class ActionRequestDto
    {

        [JsonPropertyName("Type")]
        public string Type => "ActionRequest";

        //"Move"  "Pickup"  "Drop"  "UsePotion", etc
        public string Action { get; set; } = string.Empty;

        public string? Direction { get; set; } // for "Move"
        public int? InventoryIndex { get; set; } // for "Equip" / "Drop" / "UsePotion"
        public int? HandNumber { get; set; } // for "Equip" / "Unequip"
        public string? ItemName { get; set; } // optional (not used by server)
    }
}
