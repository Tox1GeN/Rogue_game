using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue
{
    public class GameState
    {
        public Room Dungeon { get; set; }         // 20x40 grid of Cell objects
        public List<Player> Players { get; set; }  // All players in the game
        public List<Enemy> Enemies { get; set; }   // All enemies in the dungeon
        public int CurrentPlayerTurnIndex { get; set; } = 0;  // whose turn (index in Players list)
        public int MovesRemaining { get; set; } = 5;          // moves left in current player's turn

        // Other game state fields as needed (e.g., turn number, etc.)
    }
}
