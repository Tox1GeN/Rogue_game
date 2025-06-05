using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core;
using Rogue.Network.Dto;
using Rogue.Models.Interfaces;
using System.Collections.Generic;

namespace Rogue.Models.EnemyBehaviour
{
    // Calm behavior: enemy does nothing on its turn unless provoked.
    public class CalmBehaviour : IEnemyBehavior
    {
        public void ExecuteBehavior(Enemy enemy, int enemyRow, int enemyCol, Room room, IEnumerable<Player> players, GameUpdateDTO update)
        {
            // Calm enemies do not move or attack on their turn.
        }
    }
}
