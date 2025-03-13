using Rogue.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models.Currency
{
    public class Coin : Item
    {
        public int Count { get; private set; }
        public Coin(int count)
        {
            Count = count;
        }

        // Toss a coin to your Witcher O' Valley of Plenty ...
        public void CoinUp()
        {
            Count++;
        }
        public override void PickUp(Player player, Room currentRoom)
        {
            player.Coins.CoinUp();
        }
    }
}
