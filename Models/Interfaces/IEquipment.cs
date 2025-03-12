using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Interfaces
{
    public interface IEquipment : IItem
    {
        void ModifyDamage(int buffOrNerf) { }
        void ModifyArmor(int buffOrNerf) { }
    }
}
