using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Models;
using Rogue.Models.Weapons;
using Rogue.UI;
using System.Numerics;
using System.Globalization;
using System.Threading;

using Rogue.Network.Client;
using Rogue.Network.Server;

namespace Rogue
{
    internal class Program
    {
        static void Main(string[] args)
        {

            RenderDispatcher.OnRenderEvent += (IRenderEvent ev) =>
            {
                switch (ev)
                {
                    case RenderActionMessageEvent msgEv:
                        Render.Instance.DrawActionMessage(msgEv.Lines);
                        break;

                    case RedrawCellEvent cellEv:
                        Render.Instance.RedrawCell(cellEv.Row, cellEv.Col, cellEv.Room);
                        break;

                    case RenderSidePanelEvent sideEv:
                        Render.Instance.RenderSidePanel(sideEv.Player, sideEv.CurrentRoom);
                        break;

                    case RenderMonsterPanelEvent monsterEv:
                        Render.Instance.RenderMonsterPanel(monsterEv.Player, monsterEv.CurrentRoom);
                        break;

                    case RenderInstructionsEvent insEv:
                        Render.Instance.RenderInstructions(insEv.Instructions);
                        break;
                    case RequestTextInputEvent inputEv:
                        Render.Instance.DrawActionMessage(new[] { inputEv.Prompt });

                        string? input = Console.ReadLine();
                        inputEv.OnInputReceived(input);
                        break;
                }
            };

            // server || client
            if (args.Length > 0 && args[0].Equals("--server", StringComparison.OrdinalIgnoreCase))
            {
                int port = (args.Length > 1 && int.TryParse(args[1], out int p)) ? p : 5555;
                new Server(port).Run();
                return;
            }

            if (args.Length > 0 && args[0].Equals("--client", StringComparison.OrdinalIgnoreCase))
            {
                string[] addrParts = (args.Length > 1 ? args[1] : "127.0.0.1:5555").Split(':');
                string host = addrParts[0];
                int port = addrParts.Length > 1 && int.TryParse(addrParts[1], out int p) ? p : 5555;
                new Client(host, port).Connect();
                return;
            }

            // No args or manual selection: ask user interactively
            Console.WriteLine("Start as (S)erver, (C)lient, or (L)ocal singleplayer?");
            char choice = Char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (choice == 'S')
            {
                Console.Write("Enter port (default 5555): ");
                string portInput = Console.ReadLine() ?? "";
                int port = (portInput == "") ? 5555 : int.Parse(portInput);
                new Server(port).Run();
            }
            else if (choice == 'C')
            {
                Console.Write("Enter server address (default 127.0.0.1:5555): ");
                string addrInput = Console.ReadLine() ?? "";
                string[] addrParts = (addrInput == "" ? "127.0.0.1:5555" : addrInput).Split(':');
                string host = addrParts[0];
                int port = addrParts.Length > 1 && int.TryParse(addrParts[1], out int p) ? p : 5555;
                new Client(host, port).Connect();
            }
            else
            {
                // Singleplayer
                var game = new Game();
                game.Run();
            }

            //Game game = new Game();
            //game.Run();
        }
    }
}
