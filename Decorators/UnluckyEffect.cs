using Rogue.Models;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Decorators
{
    public class UnluckyEffect : EquipmentDecorator
    {
        public UnluckyEffect(Equipment equipmentToWrap) : base(equipmentToWrap) { }

        public override string GetDisplayName() => $"{wrappedEquipment.GetDisplayName()} (Unlucky)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            player.Luck -= 5;
            MessageBuffer.Add("Luck decreased by 5.");
        }

        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Luck += 5;
            MessageBuffer.Add("Luck restored by 5");
        }
    }
}
