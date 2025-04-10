using Rogue.Models;
using Rogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.Core.Generation.Interfaces;

namespace Rogue.Core.Generation.Builders
{
    public class InstructionBuilder : IBuilder
    {
        private readonly List<string> _instructions = new List<string>();
        private BuildResult _result = new BuildResult();

        public IBuilder InitGrid(int rows, int cols) { return this; }
        public IBuilder EmptyDungeon() { return this; }
        public IBuilder FilledDungeon() { return this; }
        public IBuilder AddChambers() { return this; }
        public IBuilder AddPaths() { return this; }
        public IBuilder AddCentralRoom() { return this; }

        public IBuilder AddItems()
        {
            if (!_instructions.Contains($"Press {Controls.PickUpKey.ToString()} to pick up items."))
                _instructions.Add($"Press {Controls.PickUpKey.ToString()} to pick up items.");
            if (!_instructions.Contains($"Press {Controls.DropKey.ToString()} to drop an item."))
                _instructions.Add($"Press {Controls.DropKey.ToString()} to drop an item.");
            return this;
        }

        public IBuilder AddWeapons()
        {
            if (!_instructions.Contains($"Press {Controls.DropKey.ToString()} to drop an item."))
                _instructions.Add($"Press {Controls.DropKey.ToString()} to drop an item.");
            if (!_instructions.Contains($"Press {Controls.PickUpKey.ToString()} to pick up items."))
                _instructions.Add($"Press {Controls.PickUpKey.ToString()} to pick up items.");
            if (!_instructions.Contains($"Press {Controls.EquipKey.ToString()} to equip an item from inventory."))
                _instructions.Add($"Press {Controls.EquipKey.ToString()} to equip an item from inventory.");
            if (!_instructions.Contains($"Press {Controls.UnEquipKey.ToString()} to unequip held items."))
                _instructions.Add($"Press {Controls.UnEquipKey.ToString()} to unequip held items.");
            return this;
        }

        public IBuilder AddPotions()
        {
            if (!_instructions.Contains($"Press {Controls.PickUpKey.ToString()} to pick up items."))
                _instructions.Add($"Press {Controls.PickUpKey.ToString()} to pick up items.");

            if (!_instructions.Contains($"Press {Controls.DropKey.ToString()} to drop an item."))
                _instructions.Add($"Press {Controls.DropKey.ToString()} to drop an item.");

            if (!_instructions.Contains($"Press {Controls.UseKey.ToString()} to use potions."))
                _instructions.Add($"Press {Controls.UseKey.ToString()} to use potions.");
            return this;
        }

        public IBuilder AddEnemies() { return this; }
        public IBuilder PlacePlayer(int x, int y) { return this; }

        public IBuilder EnsureConnectivity()
        {
            return this;
        }

        public IBuilder AddMovement()
        {
            // No movement handler needed in instructions.
            return this;
        }

        public BuildResult GetResult()
        {
            _result.Instructions.AddRange(_instructions);
            return _result;
        }
    }
}
