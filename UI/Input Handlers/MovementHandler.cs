using Rogue.Core;
using Rogue.Core.Combat;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI.Input_Handlers
{
    public class MovementHandler : InputHandler
    {
        public override void Handle(ConsoleKey key, Player player, Room currentRoom)
        {
            switch (key)
            {
                case Controls.UpKey:
                    TryStep(player, currentRoom, -1, 0);  // move up
                    return;  // handled, stop chain
                case Controls.DownKey:
                    TryStep(player, currentRoom, 1, 0);  // move down
                    return;
                case Controls.LeftKey:
                    TryStep(player, currentRoom, 0, -1);  // move left
                    return;
                case Controls.RightKey:
                    TryStep(player, currentRoom, 0, 1); ;   // move right
                    return;
            }

            // If not one of the movement keys, pass it to the next handler in the chain.
            base.Handle(key, player, currentRoom);
        }

        private static void TryStep(Player player, Room room, int dRow, int dCol)
        {
            var (row, col) = room.PlayerPosition;
            int newRow = row + dRow, newCol = col + dCol;

            // bounds & walls
            if (newRow < 0 || newRow >= room.Rows || newCol < 0 || newCol >= room.Columns)
                return;
            Cell targetCell = room.Grid[newRow, newCol];
            if (targetCell.IsWall) return;

            // ENEMY PRESENT  →  start combat instead of moving
            if (targetCell.Enemy != null)
            {
                targetCell.Enemy.Accept(
                    new CombatInitiationVisitor(player, room, newRow, newCol));
                return; // movement cancelled – combat took over
            }

            // empty floor or items → normal move
            player.Move(dRow, dCol, room);
        }
    }
}
