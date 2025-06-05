using Rogue.Core;
using Rogue.Models.Combat.Visitors;
using Rogue.Models.EnemyBehaviour;
using Rogue.Models.Interfaces;
using Rogue.Models.UsableItems.Potions;
using Rogue.Network.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    public class Enemy
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int AttackPower { get; private set; }

        // The current behavior strategy of this enemy
        public IEnemyBehavior Behavior { get; set; }
        // Enemy's position in the dungeon grid (Row, Col)
        public (int Row, int Col) Position { get; set; }
        // Flag to indicate if enemy already acted this round (to prevent double moves)
        public bool HasActedThisRound { get; set; }

        // Has the aggressive behavior been triggered (player entered 3×3)?
        public bool IsProvoked { get; set; }


        // Store the maximum (initial) health for threshold calculations
        private int _maxHealth;

        public Enemy(string name, int health, int attackPower, IEnemyBehavior behaviour)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            _maxHealth = health;
            Behavior = behaviour;
            HasActedThisRound = false;
            IsProvoked = false;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0)
                Health = 0;
            // If enemy survives damage, possibly change behavior based on provocation or low health
            if (Health > 0)
            {
                bool lowHealth = Health <= _maxHealth / 3;
                if (Behavior is CalmBehaviour && damage > 0)
                {
                    // Provoked: calm enemy becomes aggressive (or cowardly if critically low)
                    Behavior = lowHealth ? new CowardlyBehaviour() : new AggressiveBehaviour();
                    IsProvoked = true;
                }
                else if (!(Behavior is CowardlyBehaviour) && lowHealth)
                {
                    // Health fell low: switch aggressive (or any non-cowardly) enemy to cowardly
                    Behavior = new CowardlyBehaviour();
                    IsProvoked = false;
                }
            }
        }

        public override string ToString() => Name;
        public void Accept(IEnemyVisitor visitor) => visitor.VisitEnemy(this);

        public void TakeTurn(Room room, IEnumerable<Player> players, GameUpdateDTO update)
        {
            var (r, c) = Position;
            Behavior.ExecuteBehavior(this, r, c, room, players, update);
        }
    }
}
