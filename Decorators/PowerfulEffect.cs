using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    public class PowerfulEffect : EquipmentDecorator
    {
        public PowerfulEffect(Equipment equipmentToWrap) : base(equipmentToWrap)
        {
            wrappedEquipment.ModifyDamage(3);
        }

        public override string GetDisplayName() => $"{wrappedEquipment.GetDisplayName()} (Powerful)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            Console.WriteLine("Damage of this weapon is increased by 2.");
        }

        //public override void Unequip(Player player)
        //{
        //    base.Unequip(player);
        //}
    }
}
