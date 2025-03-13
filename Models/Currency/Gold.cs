using Rogue.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Currency
{
    public class Gold : Item
    {
        public int Count { get; private set; }
        public Gold(int count)
        {
            Count = count;
        }
        public void GoldUp()
        {
            Count++;
        }
        public override void PickUp(Player player, Room currentRoom)
        {
            player.GoldValue.GoldUp();
        }
    }
}
