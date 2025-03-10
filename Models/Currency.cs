using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Models
{
    internal enum CurrencyType { Coin, Gold }
    internal class Currency : Item
    {
        // Private set for value and the type to liquidate the cheating:
        // - money duplication;
        // - changing the currency to multiply the value of money
        public int Value { get; private set; }
        public CurrencyType Type { get; private set; }

        public Currency(CurrencyType type, int value)
        {
            Type = type;
            Value = value;
            Name = type.ToString();
        }

        public override string GetDisplayName() => $"{Name} ({Value})";

        // NO need to override Equip and Unequip methods
    }
}
