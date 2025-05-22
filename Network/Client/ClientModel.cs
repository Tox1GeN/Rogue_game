using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Network.Dto;

namespace Rogue.Network.Client
{
    public class ClientModel
    {
        public int Rows { get; }
        public int Cols { get; }

        // Map representation (character and color for each cell)
        private readonly char[,] _mapSymbols;
        private readonly ConsoleColor[,] _mapColors;

        // Known players and enemies in the game
        public Dictionary<int, PlayerDTO> Players { get; } = new();
        public Dictionary<int, EnemyDTO> Enemies { get; } = new();

        // Local player's identity, inventory and equipment
        public int LocalPlayerId { get; }
        public PlayerDTO LocalPlayer => Players[LocalPlayerId];
        public List<string> Inventory { get; } = new List<string>();
        public string?[] Equipped { get; } = new string?[2];

        // Action log lines (most recent first, max 10 lines displayed)
        public List<string> LogLines { get; } = new List<string>();
        // Turn management
        public int CurrentTurnPlayerId { get; set; }
        public int MovesRemaining { get; set; }
        public bool IsLocalPlayersTurn => CurrentTurnPlayerId == LocalPlayerId;
        // Pending combat choice request
        public bool AwaitAttackChoice { get; set; } = false;
        public AttackChoiceRequestDto? PendingAttackRequest { get; set; } = null;

        public ClientModel(GameStateDTO dto)
        {
            Rows = dto.Rows;
            Cols = dto.Cols;

            _mapSymbols = new char[Rows, Cols];
            _mapColors = new ConsoleColor[Rows, Cols];
        }
    }
}
