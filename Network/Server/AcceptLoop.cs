using Rogue.Models;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Rogue.Network.Dto;
using Rogue.Network.Dto.Events;

namespace Rogue.Network.Server
{
    public partial class Server
    {
        private TcpListener? _listener;
        private int _nextPlayerId = 1;
        private void StartListenerThread()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            new Thread(AcceptLoop) { IsBackground = true }.Start();
        }

        private void AcceptLoop()
        {
            while (true)
            {
                TcpClient tcp = _listener!.AcceptTcpClient();
                tcp.NoDelay = true;

                // create player
                int id = _nextPlayerId++;
                Player player = CreateNewPlayer(id);

                // spawn on map
                (int r, int c) = FindSpawn(_gameState.Dungeon);
                _gameState.Dungeon.Grid[r, c].PlayerOccupant = player;
                player.Position = (r, c);
                _gameState.Players.Add(player);

                // remember connection
                var conn = new ConnectedClient(tcp, player);
                _clients[player.Id] = conn;

                // send full initial state to newcomer
                var initDto = BuildInitialDto(player.Id);
                SendRaw(conn.Stream, JsonSerializer.Serialize(initDto));

                // inform others
                var joinUpd = new GameUpdateDTO();
                joinUpd.ChangedCells.Add(MakeCell(_gameState.Dungeon, r, c));
                joinUpd.Events.Add(new PlayerJoinedEvent
                {
                    PlayerId = player.Id,
                    Name = player.Nickname
                });

                Broadcast(JsonSerializer.Serialize(joinUpd));

                // if this is the first player, start the game
                if (_gameState.Players.Count == 1)
                {
                    var turnUpd = new GameUpdateDTO();
                    turnUpd.TurnInfo = new TurnDTO
                    {
                        PlayerId = player.Id,
                        PlayerIndex = 0,
                        MovesRemaining = _gameState.MovesRemaining
                    };
                    turnUpd.Events.Add(new TurnChangedEvent
                    {
                        PlayerId = player.Id,
                        MovesRemaining = _gameState.MovesRemaining
                    });
                    Broadcast(JsonSerializer.Serialize(turnUpd));
                }

                // start per-client receive loop
                new Thread(() => ClientLoop(conn)) { IsBackground = true }.Start();
            }
        }

        private Player CreateNewPlayer(int id)
        {
            ConsoleColor[] palette =
            {
                ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Green,
                ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Blue,
                ConsoleColor.DarkCyan, ConsoleColor.DarkGreen, ConsoleColor.DarkYellow
            };

            return new Player
            {
                Id = id,
                Nickname = $"Player{id}",
                Color = palette[(id - 1) % palette.Length]
            };
        }

        public void AttachPlayerToMap(Player player)
        {
            (int row, int col) = FindSpawn(_gameState.Dungeon!);
            _gameState.Dungeon.Grid[row, col].PlayerOccupant = player;
            player.Position = (row, col);

            //TODO: decide what to do with IsPlayerHere

            _gameState.Players.Add(player);
        }

        private void SendInitialDto(Player forPlayer)
        {
            GameStateDTO dto = BuildInitialDto(forPlayer.Id);


            // TODO: send json
        }
    }
}
