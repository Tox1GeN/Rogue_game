using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Concurrent;
using Rogue.Models;

namespace Rogue.Network.Server
{
    public sealed partial class Server
    {
        private readonly int _port;
        private GameState _gameState = default!;

        public Server(int port ) => _port = port;

        // Connected clients keyed by player-Id
        private readonly ConcurrentDictionary<int, ConnectedClient> _clients =
            new ConcurrentDictionary<int, ConnectedClient>();

        // Combats waiting for player to choose attack type
        private readonly ConcurrentDictionary<int, PendingCombat> _pending =
            new ConcurrentDictionary<int, PendingCombat>();

        private readonly object _stateLock = new object();

        public void Run()
        {
            // TODO:
            // 1. Build dungeon + initial GameState
            // 2. Start TcpListener
            // 3. Accept clients, spin threads, etc.
            // 4. Add try-catch-finally

            // First Step
            Console.WriteLine($"Server listening on {_port}…");
            _gameState = BuildInitialState();

            // Second & Third Steps
            StartListenerThread();

            // Block the main thread
            Thread.Sleep(Timeout.Infinite);

        }

        // Tiny container for connection stuff
        private sealed class ConnectedClient
        {
            public TcpClient Tcp        { get; }
            public NetworkStream Stream { get; }
            public Player Player        { get; }

            public ConnectedClient(TcpClient tcp, Player p)
            {
                Tcp     = tcp;
                Player  = p;
                Stream  = tcp.GetStream();
            }
        }
    }
}
