using Rogue.Models.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Combat.Visitors
{
    public interface IPlayerAttackVisitor
    {
        void VisitHeavyWeapon(Weapon weapon);
        void VisitLightWeapon(Weapon weapon);
        void VisitMagicWeapon(Weapon weapon);
        int Damage { get; }
    }
}
