using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;
using Rogue.UI;

namespace Rogue.Models
{
    public abstract class Equipment : Item, IEquipment
    {
        public override string GetDisplayName() => Name;
        public override bool CanEquip => true;
        public override void Equip(Player player)
        {
            // In decorators change a value of a player characteristic here
            Render.Instance.AddActionLine($"You've equipped the \"{GetDisplayName()}\"."); // {ShortReview}");
        }

        public override void Unequip(Player player)
        {
            // In decorators undo the changes of the value of the player characteristic here
            Render.Instance.AddActionLine($"You've unequipped the \"{GetDisplayName()}\"."); // {SecretMessage}");
        }

        public virtual void ModifyDamage(int buffOrNerf) { }
        public virtual void ModifyArmor(int buffOrNerf) { }
    }
}
