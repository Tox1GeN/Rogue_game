using Rogue.Core;
using Rogue.Core.Generation;
using Rogue.Models;
using Rogue.Models.Weapons;
using Rogue.UI;
using System.Numerics;

namespace Rogue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();

            //var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            //ProjectExporter.CreateSnapshot(projectRoot, "MyProjectSnapshot.txt");
        }
    }
}
