using Rogue.Decorators;
using Rogue.Models;
using Rogue.Models.UsableItems.Potions;
using Rogue.Models.Interfaces;
using Rogue.Models.UnusableItems;
using Rogue.Models.UsableItems;
using Rogue.Models.Weapons;
using Rogue.Models.Weapons.TwoHanded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Models.Effects;
using Rogue.Models.EnemyBehaviour;

namespace Rogue.Core
{
    public class Storage
    {
        private static Random _rng = new Random();

        // Weapons
        private static List<Func<Item>> WeaponFactories { get; } =
            new List<Func<Item>>()
        {
                () => new Sword("Wooden Sword", 2),
                () => new Sword("Iron Sword", 4),
                () => new Bow("Short Bow", 1, 0, 2),
                () => new Bow("Long Bow", 1, 0, 4),
                () => new MagicStaff("Arcane Staff", 4, 3, false, 6),
                () => new MagicStaff("Inferno Staff", 6, 5, true, 7),
                () => new Mace("Bone Crusher", 7)
        };

        // Potions
        private static List<Func<Item>> PotionFactories { get; } = new List<Func<Item>>()
        {
            () => new StrengthPotion("Berserk's Rage", 3, 5),
            () => new LuckPotion("Leprechaun's water", 5, 10),
            () => new DiscoveryPotion("Small Discover", 20, -1),
            () => new DiscoveryPotion("Big Discover", 50, -1),
            () => new Antidote()
        };

        // Regular Item
        private static List<Func<Item>> ItemFactories { get; } = new List<Func<Item>>()
        {
            () => new MysteriousNote("Strange Note", "It looks like a joke", "You are the chosen one."),
            () => new Rubbish("Rotten Apple", "Disgusting and useless."),
            () => new Egg("EASTER EGG", "???")
        };

        private static List<Func<Enemy>> MonsterFactories { get; } = new List<Func<Enemy>>()
        {
            // By default, spawn "Skeleton" enemies with 6 HP and 3 AttackPower.
            () => new Enemy("Skeleton", health: 15, attackPower: 3, behaviour: new AggressiveBehaviour()),
            () => new Enemy("Witch", health: 25, attackPower: 1, behaviour: new CowardlyBehaviour()),
            () => new Enemy("Wolf", health: 30, attackPower: 7, behaviour: new CalmBehaviour())
        };

    public static Item GetRandomWeapon()
        {
            int index = _rng.Next(WeaponFactories.Count);
            var baseWeapon = WeaponFactories[index]();

            return baseWeapon.TryEnchant(_rng);
        }

        public static Item GetRandomItem()
        {
            int index = _rng.Next(ItemFactories.Count);
            return ItemFactories[index]();
        }

        public static Item GetRandomPotion()
        {
            int index = _rng.Next(PotionFactories.Count);
            return PotionFactories[index]();
        }

        public static Enemy GetRandomEnemy()
        {
            int idx = _rng.Next(MonsterFactories.Count);
            return MonsterFactories[idx]();
        }
    }
}
