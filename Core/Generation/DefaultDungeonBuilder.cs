using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class DefaultDungeonBuilder : IDungeonBuilder
    {
        private Room? _room;
        private Room DefaultRoom => _room ?? throw new InvalidOperationException("Room is not initialized.");

        public void InitGrid(int rows, int cols)
        {
            _room = new Room(rows, cols, skipGeneration: true);
        }

        public void BuildWalls()
        {

            int maxRows = DefaultRoom.Rows;
            int maxCols = DefaultRoom.Columns;

            for (int i = 3; i < maxCols; i++)
            {
                DefaultRoom.Grid[0, i].IsWall = true;
            }

            for(int i = 0; i < maxCols; i++)
            {
                DefaultRoom.Grid[maxRows, i].IsWall = true;
            }

            for (int i = 3; i < maxRows; i++)
            {
                DefaultRoom.Grid[i, 0].IsWall = true;
            }

            for(int i = 0; i < maxRows; i++)
            {
                DefaultRoom.Grid[i, maxCols].IsWall = true;
            }
        }

        public void AddPath()
        {
            // No need for an implementation for Default Level.
        }
            
        public void AddCentralRoom()
        {
            // No need for an implementation for Default Level.
        }

        public void AddItems()
        {

            var note = new Rogue.Models.UnusableItems.MysteriousNote("Strange Note", "A old piece of paper that I found after I've woke up", "This is a note. It says: 'You are the chosen one.'");

            DefaultRoom.Grid[5, 5].Items.Push(note);
        }

        public void AddWeapons()
        {

            var sword = new Rogue.Models.Weapons.Sword("Excalibur", damage: 10);

            DefaultRoom.Grid[2, 10].Items.Push(new Rogue.Decorators.LegendaryEffect(sword));
        }


        public void PlacePlayer()
        {

            DefaultRoom.PlayerPosition = (0, 0);
            DefaultRoom.Grid[0, 0].IsPlayerHere = true;
        }

        public Room GetResult() => DefaultRoom;
    }
}
