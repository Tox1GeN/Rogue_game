using Rogue.Models;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    public class ProtectiveEffect : EquipmentDecorator
    {
        public ProtectiveEffect(Equipment equipmentToWrap) : base(equipmentToWrap) 
        {
            wrappedEquipment.ModifyArmor(3);
        }

        public override string GetDisplayName() => $"{wrappedEquipment.GetDisplayName()} (Protective)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            Render.AddActionLine("Armor of this equipment is increased by 3.");
        }
    }
}
