using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Interfaces;
using Rogue.UI;
using Rogue.Decorators;

namespace Rogue.Models
{
    public abstract class Equipment : Item, IEquipment
    {
        public override string GetDisplayName() => Name;
        public override bool CanEquip => true;
        public override void Equip(Player player)
        {
            // In decorators change a value of a player characteristic here
            MessageBuffer.Add($"You've equipped the \"{GetDisplayName()}\"."); // {ShortReview}");
        }

        public override void Unequip(Player player)
        {
            // In decorators undo the changes of the value of the player characteristic here
            MessageBuffer.Add($"You've unequipped the \"{GetDisplayName()}\"."); // {SecretMessage}");
        }

        public virtual void ModifyDamage(int buffOrNerf) { }
        public virtual void ModifyArmor(int buffOrNerf) { }

        public override Item TryEnchant(Random rng)
        {
            double chance = rng.NextDouble();
            if(chance < 0.6) return this;
            if (chance < 0.75) return new PowerfulEffect(this);
            if (chance < 0.9) return new ProtectiveEffect(this);
            if (chance < 0.97) return new LegendaryEffect(this);
            return new UnluckyEffect(this);
        }
    }
}
