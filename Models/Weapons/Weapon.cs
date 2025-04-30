using Rogue.Models.Combat.Visitors;
using Rogue.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Weapons
{
    public abstract class Weapon : Equipment, IWeapon
    {
        public abstract int Damage { get; }
        public override bool TwoHanded => false;
        protected int _damage;
        public override void ModifyDamage(int buffOrNerf)
        {
            _damage += buffOrNerf;
        }
        public override Weapon? AsWeapon() => this;
        public abstract void Accept(IPlayerAttackVisitor visitor);
        public virtual string GetDisplayDmg() => $"Damage: {Damage}";
    }
}
