using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.UI;

namespace Rogue.Decorators
{
    public class LegendaryEffect : EquipmentDecorator
    {
        public LegendaryEffect(Equipment equipmentToWrap) : base(equipmentToWrap)
        {
            wrappedEquipment.ModifyDamage(10);
        }
        public override string GetDisplayName() => $"{wrappedEquipment.GetDisplayName()} (Legendary)";

        public override void Equip(Player player)
        {
            base.Equip(player);
            player.Health += 2;
            player.Wisdom += 2;
            player.Dexterity += 2;
            player.Luck += 2;
            player.Strength += 2;

            MessageBuffer.Add("All the stats are increased by 2. Damage of this weapon is increased by 10");
        }

        public override void Unequip(Player player)
        {
            base.Unequip(player);
            player.Health -= 2;
            player.Wisdom -= 2;
            player.Dexterity -= 2;
            player.Luck -= 2;
            player.Strength -= 2;

            MessageBuffer.Add("All the stats are decreased by 2.");
        }
    }
}
