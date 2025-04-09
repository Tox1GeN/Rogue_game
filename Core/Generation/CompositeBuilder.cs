using Rogue.Core.Generation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Core.Generation
{
    public class CompositeBuilder : IBuilder
    {
        private readonly List<IBuilder> _builders = new List<IBuilder>();

        public CompositeBuilder AddBuilder(IBuilder builder)
        {
            _builders.Add(builder);
            return this;
        }

        public IBuilder InitGrid(int rows, int cols)
        {
            foreach (var builder in _builders)
                builder.InitGrid(rows, cols);
            return this;
        }

        public IBuilder EmptyDungeon()
        {
            foreach (var builder in _builders)
                builder.EmptyDungeon();
            return this;
        }

        public IBuilder FilledDungeon()
        {
            foreach (var builder in _builders)
                builder.FilledDungeon();
            return this;
        }

        public IBuilder AddChambers()
        {
            foreach (var builder in _builders)
                builder.AddChambers();
            return this;
        }

        public IBuilder AddPaths()
        {
            foreach (var builder in _builders)
                builder.AddPaths();
            return this;
        }

        public IBuilder AddCentralRoom()
        {
            foreach (var builder in _builders)
                builder.AddCentralRoom();
            return this;
        }

        public IBuilder AddItems()
        {
            foreach (var builder in _builders)
                builder.AddItems();
            return this;
        }

        public IBuilder AddWeapons()
        {
            foreach (var builder in _builders)
                builder.AddWeapons();
            return this;
        }

        public IBuilder AddPotions()
        {
            foreach (var builder in _builders)
                builder.AddPotions();
            return this;
        }

        public IBuilder AddEnemies()
        {
            foreach (var builder in _builders)
                builder.AddEnemies();
            return this;
        }

        public IBuilder PlacePlayer(int x, int y)
        {
            foreach (var builder in _builders)
                builder.PlacePlayer(x, y);
            return this;
        }

        public IBuilder EnsureConnectivity()
        {
            foreach (var builder in _builders)
                builder.EnsureConnectivity();
            return this;
        }

        public IBuilder AddMovement()
        {
            foreach (var builder in _builders)
                builder.AddMovement();
            return this;
        }

        public BuildResult GetResult()
        {
            BuildResult compositeResult = new BuildResult();
            foreach (var builder in _builders)
            {
                BuildResult result = builder.GetResult();
                if (result.Dungeon != null)
                    compositeResult.Dungeon = result.Dungeon;
                if (result.Instructions != null)
                    compositeResult.Instructions.AddRange(result.Instructions);
                if (result.InputHandlers != null)
                    compositeResult.InputHandlers.AddRange(result.InputHandlers);
            }
            return compositeResult;
        }
    }
}
