using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue;
using Rogue.Models;

namespace Rogue.Decorators
{
    public class EquipmentDecorator : Equipment
    {
        protected Equipment wrappedEquipment;

        // Constructor of Decorator
        public EquipmentDecorator(Equipment equipmentToWrap)
        {
            wrappedEquipment = equipmentToWrap;
        }

        public override string GetDisplayName() => wrappedEquipment.GetDisplayName();

        public override void Equip(Player player) => wrappedEquipment.Equip(player);

        public override void Unequip(Player player) => wrappedEquipment.Unequip(player);

        public override void ModifyDamage(int buffOrNerf)
        {
            wrappedEquipment.ModifyDamage(buffOrNerf);
        }

        public override void ModifyArmor(int buffOrNerf)
        {
            wrappedEquipment.ModifyArmor(buffOrNerf);
        }
    }
}
