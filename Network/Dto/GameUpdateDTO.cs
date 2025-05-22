using Rogue.Network.Dto.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto
{

    public class GameUpdateDTO
    {
        public List<CellUpdateDTO> ChangedCells { get; set; } = new();
        public List<BaseEvent> Events { get; set; } = new();
        public TurnDTO? TurnInfo { get; set; }
    }

    public class CellUpdateDTO
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }
        // We include symbol & color for convenience so the client can directly redraw, 
        // but we might also need to update local state:
        public bool NowHasPlayer { get; set; }
        public int? PlayerId { get; set; }
        public bool NowHasEnemy { get; set; }
        public int? EnemyId { get; set; }
        public bool IsWall { get; set; }  // likely constant once set, but just in case
        public bool NowHasItem { get; set; }
        public char? ItemSymbol { get; set; }
    }

    public class TurnDTO
    {
        public int PlayerId { get; set; }
        public int PlayerIndex { get; set; }
        public int MovesRemaining { get; set; }
    }
    //public class GameUpdateDTO
    //{
    //    public List<CellUpdateDTO> ChangedCells { get; set; } = new List<CellUpdateDTO>();
    //    public List<PlayerStatusDTO> UpdatedPlayers { get; set; } = new List<PlayerStatusDTO>();
    //    public List<string> Messages { get; set; } = new List<string>();
    //    public TurnDTO? TurnInfo { get; set; } = null;
    //}
    //public class PlayerStatusDTO
    //{
    //    public int Id { get; set; }
    //    public int Health { get; set; }
    //    public int Coins { get; set; }
    //    // ... other stats if they can change, like if potion boosts Strength, include that too.
    //    public List<string> Inventory { get; set; }  // maybe list item names in inventory after a change
    //}
}
