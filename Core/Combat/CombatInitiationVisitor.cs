using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models;
using Rogue.Core.Combat;
using Rogue.Models.Combat.Visitors;
using Rogue.UI;

namespace Rogue.Core.Combat
{
    // called from MovementHandler when the player tries to enter a cell with an enemy
    public sealed class CombatInitiationVisitor : IEnemyVisitor
    {
        private readonly Player _player;
        private readonly Room _room;
        private readonly int _destRow, _destCol;
        public CombatInitiationVisitor(Player p, Room r, int row, int col)
        { _player = p; _room = r; _destRow = row; _destCol = col; }

        public void VisitEnemy(Enemy enemy)
        {
            new CombatSession(_player, enemy, _room).Start();
            if (enemy.Health <= 0 && _player.Health > 0)
            {
                // move player into the slain enemy’s cell
                var (cr, cc) = _room.PlayerPosition;
                _room.Grid[cr, cc].IsPlayerHere = false;
                _room.Grid[_destRow, _destCol].IsPlayerHere = true;
                _room.Grid[_destRow, _destCol].Enemy = null;
                _room.PlayerPosition = (_destRow, _destCol);

                RepaintAfterCombat(cr, cc, _destRow, _destCol);
            }
        }

        private void RepaintAfterCombat(int oldR, int oldC, int newR, int newC)
        {
            Render.Instance.RedrawCell(oldR, oldC, _room);   // where player was
            Render.Instance.RedrawCell(newR, newC, _room);   // where player is

            // If the enemy glyph was drawn with different coords (rare), clear it too.
            if (oldR != newR || oldC != newC)
                Render.Instance.RedrawCell(newR, newC, _room);
        }
    }
}
