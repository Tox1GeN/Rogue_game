using Rogue.Models;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rogue.Network.Server
{
    public sealed partial class Server
    {
        private void ClientLoop(ConnectedClient cli)
        {
            var reader = new StreamReader(cli.Stream, Encoding.UTF8);
            try
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    // choose action by "Type" discriminator
                    if (line.Contains("\"Type\":\"AttackChoiceResponse\""))
                    {
                        var dto = JsonSerializer.Deserialize<AttackChoiceResponseDto>(line);
                        HandleAttackChoice(dto!, cli.Player);
                    }
                    else
                    {
                        var action = JsonSerializer.Deserialize<ActionRequestDto>(line);
                        HandleAction(action!, cli.Player);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[Server] {ex.Message}"); }
            finally { RemoveClient(cli); }
        }

        private void RemoveClient(ConnectedClient cli)
        {
            Console.WriteLine($"[Server] {cli.Player.Nickname} disconnected.");
            _clients.TryRemove(cli.Player.Id, out _);

            lock (_stateLock)
            {
                var cell = _gameState.Dungeon.Grid[cli.Player.Position.Row,
                                                   cli.Player.Position.Col];
                cell.PlayerOccupant = null;
                _gameState.Players.Remove(cli.Player);

                var upd = new GameUpdateDTO();
                upd.ChangedCells.Add(MakeCell(_gameState.Dungeon, cli.Player.Position.Row, cli.Player.Position.Col));
                upd.Events.Add(new PlayerLeftEvent
                {
                    PlayerId = cli.Player.Id,
                    Name = cli.Player.Nickname
                });
                Broadcast(JsonSerializer.Serialize(upd));

                // adjust turn index if necessary
                if (_gameState.CurrentPlayerTurnIndex >= _gameState.Players.Count)
                    _gameState.CurrentPlayerTurnIndex = 0;
            }
            cli.Tcp.Close();
        }
    }
}
