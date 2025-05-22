using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Network.Dto
{
    public class GameStateDTO
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        // Only non-empty / non-floor cells to keep JSON small
        public List<CellDTO> Cells { get; set; } = new();
        public List<PlayerDTO> Players { get; set; } = new();
        public List<EnemyDTO> Enemies { get; set; } = new();

        public int YourPlayerId { get; set; }
    }

    public sealed class CellDTO
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public bool IsWall { get; set; }

        public char? ItemSymbol { get; set; }
        public string? ItemType { get; set; }

        public char? EnemySymbol { get; set; }
    }

    public sealed class PlayerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Row { get; set; }
        public int Col { get; set; }
        public ConsoleColor Color { get; set; }

        public int Health { get; set; }
        public int Strength { get; set; }
        // TODO:
        // add Dex, Luck, etc. 
    }

    public sealed class EnemyDTO
    {
        public int Id { get; set; }          // optional
        public string Name { get; set; } = "";
        public char Symbol { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }

        public int Health { get; set; }
        // attack power or other fields if the client needs them
    }
}
