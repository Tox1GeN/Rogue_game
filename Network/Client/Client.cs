using Rogue.Core;
using Rogue.Models;
using Rogue.Models.EnemyBehaviour;
using Rogue.Models.UnusableItems;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rogue.Network.Client
{
    public sealed class Client
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;
        private TcpClient _tcp;
        private StreamReader _reader;
        private StreamWriter _writer;

        private int _localPlayerId;
        private Player _localPlayer;
        private Room _room;

        // Track all players and enemies for model and display.
        private Dictionary<int, Player> _players = new();
        private Dictionary<int, Enemy> _enemies = new();

        // If waiting for combat choice, main input is suspended.
        private bool _awaitingAttackChoice = false;

        public Client(string address, int port)
        {
            _serverAddress = address;
            _serverPort = port;
        }

        public void Connect()
        {
            // 1. Connect TcpClient
            _tcp = new TcpClient(_serverAddress, _serverPort);
            _reader = new StreamReader(_tcp.GetStream());
            _writer = new StreamWriter(_tcp.GetStream()) { AutoFlush = true };

            // 2. Receive GameStateDTO (initial full state)
            string initJson = _reader.ReadLine();
            var initDto = JsonSerializer.Deserialize<GameStateDTO>(initJson)!;
            HandleInitialState(initDto);

            // 3. Start network-listening thread
            new Thread(ListenLoop) { IsBackground = true }.Start();

            // 4. Enter user input loop (controller)
            InputLoop();

            // Cleanup on exit
            _tcp.Close();
        }

        private void HandleInitialState(GameStateDTO dto)
        {
            // Store local player ID
            _localPlayerId = dto.YourPlayerId;

            // Create the Room with given dimensions
            _room = new Room(dto.Rows, dto.Cols);

            // Initialize all cells as walls or floors
            // (Server only sent non-empty cells; others default to floor.)
            foreach (var cellDto in dto.Cells)
            {
                var cell = _room.Grid[cellDto.Row, cellDto.Col];
                cell.IsWall = cellDto.IsWall;
                if (cellDto.IsWall) continue;

                // If there is an item, push a dummy Item for display:
                if (cellDto.ItemSymbol.HasValue)
                {
                    // Create a dummy item with that symbol (type is not needed for display)
                    Item item = new Rubbish("HiddenItem", ""); // Placeholder
                    cell.Items.Push(item);
                }
                // Enemies and players placed below.
            }

            foreach (var pDto in dto.Players)
            {
                var player = new Player
                {
                    Id = pDto.Id,
                    Nickname = pDto.Name,
                    Color = pDto.Color,
                    Health = pDto.Health,
                    Strength = pDto.Strength
                    // Additional stats (Dexterity, etc.) could be set here if provided
                };

                // Set starting position
                player.Position = (pDto.Row, pDto.Col);
                _players[player.Id] = player;

                // Place in room grid
                _room.Grid[pDto.Row, pDto.Col].PlayerOccupant = player;

                // If this is the local player, keep a reference and mark in room
                if (player.Id == _localPlayerId)
                {
                    _localPlayer = player;
                    _room.PlayerPosition = (pDto.Row, pDto.Col);
                }
            }

            // Create Enemy objects and place them
            foreach (var eDto in dto.Enemies)
            {
                var enemy = Storage.GetRandomEnemy();
                //enemy.Id = eDto.Id;
                _enemies[eDto.Id] = enemy;
                _room.Grid[eDto.Row, eDto.Col].Enemy = enemy;
            }

            Console.Clear();

            for (int r = 0; r < dto.Rows; r++)
            {
                for (int c = 0; c < dto.Cols; c++)
                {
                    RenderDispatcher.Raise(new RedrawCellEvent(r, c, _room));
                }
            }

            RenderDispatcher.Raise(new RenderSidePanelEvent(_localPlayer, _room));
            RenderDispatcher.Raise(new RenderMonsterPanelEvent(_localPlayer, _room));
        }

        private void ListenLoop()
        {
            try
            {
                string? line;
                while ((line = _reader.ReadLine()) != null)
                {
                    // Determine message type by a simple check on JSON content
                    if (line.Contains("\"Type\":\"AttackChoiceRequest\""))
                    {
                        // Combat initiated: server asks for attack type
                        var req = JsonSerializer.Deserialize<AttackChoiceRequestDto>(line)!;
                        PromptAttackChoice(req);
                    }
                    else if (line.Contains("\"Rows\"") && line.Contains("\"Cols\""))
                    {
                        // If by chance another full state arrives (not typical), handle it
                        var fullState = JsonSerializer.Deserialize<GameStateDTO>(line)!;
                        // (Might ignore or re-handle initial state)
                    }
                    else
                    {
                        // Regular game update
                        var update = JsonSerializer.Deserialize<GameUpdateDTO>(line)!;
                        ProcessGameUpdate(update);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Connection error: {ex.Message}");
            }
        }

        private void ProcessGameUpdate(GameUpdateDTO update)
        {
            foreach (var cu in update.ChangedCells)
            {
                int r = cu.Row, c = cu.Col;
                var cell = _room.Grid[r, c];
                // Update cell properties
                cell.IsWall = cu.IsWall;

                if (cu.NowHasPlayer)
                {
                    // Find or create the player
                    if (!_players.TryGetValue(cu.PlayerId.Value, out var ply))
                    {
                        // Unknown player (could happen on late join)
                        ply = new Player { Id = cu.PlayerId.Value, Nickname = $"P{cu.PlayerId.Value}" };
                        _players[ply.Id] = ply;
                    }
                    cell.PlayerOccupant = ply;
                }
                else
                {
                    cell.PlayerOccupant = null;
                }

                // Update enemy occupant
                if (cu.NowHasEnemy)
                {
                    if (!_enemies.TryGetValue(cu.EnemyId.Value, out var ene))
                    {
                        // Create new enemy stub if unknown
                        ene = new Enemy("Enemy", 0, 0, new CalmBehaviour());
                        _enemies[cu.EnemyId.Value] = ene;
                    }
                    cell.Enemy = ene;
                }
                else
                {
                    cell.Enemy = null;
                }

                // Update items on floor (we only know symbol, so use placeholder logic)
                if (cu.NowHasItem)
                {
                    // Push a dummy item onto stack for display
                    cell.Items.Push(new Rubbish("Item", ""));
                }
                else
                {
                    cell.Items.Clear();
                }

                // Redraw this cell
                RenderDispatcher.Raise(new RedrawCellEvent(r, c, _room));
            }

            MessageBuffer.Begin();

            foreach (var ev in update.Events)
            {
                switch (ev)
                {
                    case PlayerJoinedEvent joined:
                        MessageBuffer.Add($"{joined.Name} has joined the game.");
                        break;

                    case PlayerLeftEvent left:
                        MessageBuffer.Add($"{left.Name} has left the game.");
                        break;

                    case PlayerMovedEvent moved:
                        {
                            string mover = (moved.PlayerId == _localPlayerId ? "You" : _players[moved.PlayerId].Nickname);
                            MessageBuffer.Add($"{mover} moved to ({moved.ToRow},{moved.ToCol}).");
                        }
                        break;

                    case ItemPickedUpEvent pick:
                        {
                            string who = (pick.PlayerId == _localPlayerId ? "You" : _players[pick.PlayerId].Nickname);
                            MessageBuffer.Add($"{who} picked up {pick.ItemName}.");
                        }
                        break;

                    case ItemDroppedEvent drop:
                        {
                            string who = (drop.PlayerId == _localPlayerId ? "You" : _players[drop.PlayerId].Nickname);
                            MessageBuffer.Add($"{who} dropped {drop.ItemName}.");
                        }
                        break;

                    case ItemEquippedEvent eq:
                        {
                            string who = (eq.PlayerId == _localPlayerId ? "You" : _players[eq.PlayerId].Nickname);
                            string hand = (eq.HandNumber == 0 ? "left" : "right");
                            MessageBuffer.Add($"{who} equipped {eq.ItemName} in {hand} hand.");
                        }
                        break;

                    case ItemUnequippedEvent uneq:
                        {
                            string who = (uneq.PlayerId == _localPlayerId ? "You" : _players[uneq.PlayerId].Nickname);
                            MessageBuffer.Add($"{who} unequipped {uneq.ItemName}.");
                        }
                        break;

                    case PotionUsedEvent pu:
                        {
                            string who = (pu.PlayerId == _localPlayerId ? "You" : _players[pu.PlayerId].Nickname);
                            MessageBuffer.Add($"{who} used {pu.ItemName}.");
                        }
                        break;

                    case CombatResolvedEvent cr:
                        {
                            // Display combat messages
                            if (cr.PlayerId == _localPlayerId)
                            {
                                // Local player was attacker
                                if (!cr.EnemyDefeated)
                                    MessageBuffer.Add($"You dealt {cr.EnemyDamageTaken} damage to {cr.EnemyName}.");
                                else
                                    MessageBuffer.Add($"You defeated {cr.EnemyName}!");
                                if (cr.PlayerDefeated)
                                    MessageBuffer.Add("You have been defeated.");
                                else if (cr.PlayerDamageTaken > 0)
                                    MessageBuffer.Add($"{cr.EnemyName} hit you for {cr.PlayerDamageTaken} damage.");
                            }
                            else
                            {
                                // Another player was attacker
                                string pname = _players[cr.PlayerId].Nickname;
                                if (!cr.EnemyDefeated)
                                    MessageBuffer.Add($"{pname} dealt {cr.EnemyDamageTaken} damage to {cr.EnemyName}.");
                                else
                                    MessageBuffer.Add($"{pname} defeated {cr.EnemyName}.");
                                // (We typically don't know if that other player was hurt without extra data)
                            }
                        }
                        break;

                    case TurnChangedEvent turn:
                        {
                            if (turn.PlayerId == _localPlayerId)
                                MessageBuffer.Add($"Your turn ({turn.MovesRemaining} moves remaining).");
                            else
                                MessageBuffer.Add($"{_players[turn.PlayerId].Nickname}'s turn.");
                        }
                        break;

                    case PlayerStatusDto stat:
                        {
                            // Update stats of a player
                            if (stat.Id == _localPlayerId)
                            {
                                _localPlayer.Health = stat.Health;
                                _localPlayer.Strength = stat.Strength;
                                _localPlayer.Dexterity = stat.Dexterity;
                                _localPlayer.Luck = stat.Luck;
                                _localPlayer.Wisdom = stat.Wisdom;
                                // InventoryCount or Equipped list can be used if needed
                                // Then refresh side panel
                            }
                        }
                        break;

                    default:
                        // Other event types can be handled here.
                        break;
                }
            }

            MessageBuffer.Commit();
            
            bool statsChanged = update.Events.OfType<PlayerStatusDto>()
                         .Any(s => s.Id == _localPlayerId);
            if (statsChanged)
            {
                RenderDispatcher.Raise(new RenderSidePanelEvent(_localPlayer, _room));
            }
            RenderDispatcher.Raise(new RenderMonsterPanelEvent(_localPlayer, _room));
        }

        private void PromptAttackChoice(AttackChoiceRequestDto req)
        {
            _awaitingAttackChoice = true;
            RenderDispatcher.Raise(new RenderActionMessageEvent(new List<string> { $"A wild {req.EnemyName} attacks! Choose attack – (N)ormal, (S)tealth, (M)agic." }));

            // Read key (blocking here for simplicity)
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.N && key != ConsoleKey.S && key != ConsoleKey.M);

            string choice = key switch
            {
                ConsoleKey.S => "Stealth",
                ConsoleKey.M => "Magic",
                _ => "Normal",
            };

            // Send response back to server
            var resp = new AttackChoiceResponseDto
            {
                PlayerId = req.PlayerId,
                EnemyId = req.EnemyId,
                EnemyName = req.EnemyName,
                Choice = choice
            };
            string json = JsonSerializer.Serialize(resp);
            _writer.WriteLine(json);

            _awaitingAttackChoice = false;
        }

        private void InputLoop()
        {
            while (true)
            {
                if (_awaitingAttackChoice)
                {
                    Thread.Sleep(10);
                    continue;
                }

                // Read a key or command from user
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                var actionDto = new ActionRequestDto();

                switch (keyInfo.Key)
                {
                    case Controls.UpKey:
                        actionDto.Action = "Move";
                        actionDto.Direction = "Up";
                        break;
                    case Controls.DownKey:
                        actionDto.Action = "Move";
                        actionDto.Direction = "Down";
                        break;
                    case Controls.LeftKey:
                        actionDto.Action = "Move";
                        actionDto.Direction = "Left";
                        break;
                    case Controls.RightKey:
                        actionDto.Action = "Move";
                        actionDto.Direction = "Right";
                        break;

                    case Controls.PickUpKey:
                        actionDto.Action = "Pickup";
                        break;

                    case Controls.DropKey:
                        actionDto.Action = "Drop";
                        break;

                    case Controls.EquipKey:
                        actionDto.Action = "Equip";
                        break;

                    case Controls.UnEquipKey:
                        actionDto.Action = "Unequip";
                        break;

                    case Controls.UseKey:
                        actionDto.Action = "UsePotion";
                        break;

                    default:
                        continue;
                }

                // Serialize and send the action request
                string reqJson = JsonSerializer.Serialize(actionDto);
                _writer.WriteLine(reqJson);
            }
        }


    }
}
