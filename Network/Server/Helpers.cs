using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Core.Generation.Builders;
using Rogue.Core.Generation.Interfaces;
using Rogue.Models;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;

namespace Rogue.Network.Server
{
    public sealed partial class Server
    {

        public static GameState BuildInitialState()
        {
            var builder = new CompositeBuilder()
                .AddBuilder(new DungeonGeneration())
                .AddBuilder(new InstructionBuilder())
                .AddBuilder(new ChainBuilder());

            var director = new DungeonDirector(builder);
            var dummy = new Player();
            BuildResult buildResult = director.ConstructServerDungeon(20, 40);

            var gameState = new GameState
            {
                Dungeon = buildResult.Dungeon!,
                Enemies = new List<Enemy>(),
                Players = new List<Player>(),
                CurrentPlayerTurnIndex = 0,
                MovesRemaining = 5
            };

            // collect enemies from the Grid
            for(int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (gameState.Dungeon.Grid[i, j].Enemy != null)
                    {
                        gameState.Enemies.Add(gameState.Dungeon.Grid[i, j].Enemy!);
                    }
                }
            }

            return gameState;
        }

        private (int Row, int Col) FindSpawn(Room room)
        {
            int cx = room.Rows / 2, cy = room.Columns / 2;
            (int r, int c)[] offsets =
            { (0,0),(1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1) };

            foreach (var (dr, dc) in offsets)
            {
                int r = cx + dr, c = cy + dc;
                if (Inside(r, c) && !room.Grid[r, c].IsWall &&
                    room.Grid[r, c].PlayerOccupant == null &&
                    room.Grid[r, c].Enemy == null)
                    return (r, c);
            }
            // fallback – linear scan
            for (int r = 0; r < room.Rows; r++)
                for (int c = 0; c < room.Columns; c++)
                    if (!room.Grid[r, c].IsWall &&
                        room.Grid[r, c].PlayerOccupant == null &&
                        room.Grid[r, c].Enemy == null)
                        return (r, c);

            throw new InvalidOperationException("No free spawn tile");
        }
        private bool Inside(int r, int c) =>
            r >= 0 && r < _gameState.Dungeon.Rows &&
            c >= 0 && c < _gameState.Dungeon.Columns;

        private GameStateDTO BuildInitialDto(int yourId)
        {
            var dto = new GameStateDTO
            {
                Rows = _gameState.Dungeon.Rows,
                Cols = _gameState.Dungeon.Columns,
                YourPlayerId = yourId
            };


            // Cells information (that are not empty)
            for (int r = 0; r < dto.Rows; r++)
                for (int c = 0; c < dto.Cols; c++)
                {
                    // Skip empty ones
                    Cell cell = _gameState.Dungeon.Grid[r, c];
                    bool isEmptyFloor = !cell.IsWall &&
                                        cell.PlayerOccupant == null &&
                                        cell.Enemy == null &&
                                        cell.Items.Count == 0;

                    if (isEmptyFloor) continue;

                    var cellDto = new CellDTO
                    {
                        Row = r,
                        Col = c,
                        IsWall = cell.IsWall
                    };

                    if (cell.Items.Count > 0)
                    {
                        var top = cell.Items.Peek();
                        cellDto.ItemSymbol = top.GetDisplayName()[0];
                        cellDto.ItemType = top.GetType().Name;
                    }
                    if (cell.Enemy != null)
                    {
                        cellDto.EnemySymbol = cell.Enemy.Name[0];
                    }
                    dto.Cells.Add(cellDto);
                }

            // Players
            foreach (Player player in _gameState.Players)
            {
                dto.Players.Add(new PlayerDTO
                {
                    Id = player.Id,
                    Name = player.Nickname,
                    Row = player.Position.Row,
                    Col = player.Position.Col,
                    Color = player.Color,
                    Health = player.Health,
                    Strength = player.Strength
                });
            }

            // Enemies 
            foreach (Enemy enemy in _gameState.Enemies)
            {
                dto.Enemies.Add(new EnemyDTO
                {
                    Id = enemy.GetHashCode(),          // or a dedicated field
                    Name = enemy.Name,
                    Symbol = enemy.Name[0],
                    Row = FindEnemyRow(enemy),
                    Col = FindEnemyCol(enemy),
                    Health = enemy.Health
                });
            }

            return dto;
        }

        private int FindEnemyRow(Enemy e)
        {
            for (int r = 0; r < _gameState.Dungeon.Rows; r++)
                for (int c = 0; c < _gameState.Dungeon.Columns; c++)
                    if (_gameState.Dungeon.Grid[r, c].Enemy == e)
                        return r;
            return 0;
        }
        private int FindEnemyCol(Enemy e)
        {
            for (int r = 0; r < _gameState.Dungeon.Rows; r++)
                for (int c = 0; c < _gameState.Dungeon.Columns; c++)
                    if (_gameState.Dungeon.Grid[r, c].Enemy == e)
                        return c;
            return 0;
        }

        private static CellUpdateDTO MakeCell(Room room, int r, int c) =>
            new CellUpdateDTO
            {
                Row = r,
                Col = c,
                Symbol = room.Grid[r, c].GetDisplayCell().symbol,
                Color = room.Grid[r, c].GetDisplayCell().color,
                NowHasPlayer = room.Grid[r, c].PlayerOccupant != null,
                PlayerId = room.Grid[r, c].PlayerOccupant?.Id
            };

        private void FinishTurn()
        {
            _gameState.CurrentPlayerTurnIndex =
                (_gameState.CurrentPlayerTurnIndex + 1) % _gameState.Players.Count;
            _gameState.MovesRemaining = 5;

            var next = _gameState.Players[_gameState.CurrentPlayerTurnIndex];
            var update = new GameUpdateDTO();

            update.TurnInfo = new TurnDTO
            {
                PlayerId = next.Id,
                PlayerIndex = _gameState.CurrentPlayerTurnIndex,
                MovesRemaining = 5
            };

            update.Events.Add(new TurnChangedEvent
            {
                PlayerId = next.Id,
                MovesRemaining = 5
            });

            Broadcast(JsonSerializer.Serialize(update));
        }

        // Broadcast/send
        private void Broadcast(string json)
        {
            foreach (var c in _clients.Values)
                SendRaw(c.Stream, json);
        }
        private void SendToClient(Player p, string json)
        {
            if (_clients.TryGetValue(p.Id, out ConnectedClient c))
                SendRaw(c.Stream, json);
        }
        private static void SendRaw(NetworkStream ns, string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json + '\n');
            ns.Write(bytes, 0, bytes.Length);
        }


        // Player's snapshot

        private PlayerStatusDto MakeInvStat(Player player)
        {
            return new PlayerStatusDto
            {
                Id = player.Id,
                Health = player.Health,
                Strength = player.Strength,
                Dexterity = player.Dexterity,
                Luck = player.Luck,
                Wisdom = player.Wisdom,
                // Coins = player.Coins.Value,
                InventoryCount = player.Inventory.Items.Count,
                Equipped = new List<string>
        {
            player.Hands[0]?.GetDisplayName() ?? "",
            player.Hands[1]?.GetDisplayName() ?? ""
        }
            };
        }
    }
}
