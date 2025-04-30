using Rogue.Models.Combat.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons.TwoHanded
{
    public class Mace : TwoHandedWeapon
    {
        public Mace(string name, int damage)
        {
            Name = name;
            _damage = damage;
        }

        public override int Damage => _damage;
        public override void Accept(IPlayerAttackVisitor visitor) => visitor.VisitHeavyWeapon(this);
    }
}
