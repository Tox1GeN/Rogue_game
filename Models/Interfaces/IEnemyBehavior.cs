using Rogue.Core;
using Rogue.Network.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    // Strategy interface for enemy behaviors. Defines the enemy's action per turn.
    public interface IEnemyBehavior
    {
        void ExecuteBehavior(Enemy enemy, int enemyRow, int enemyCol, Room room, IEnumerable<Player> players, GameUpdateDTO update);
    }
}

