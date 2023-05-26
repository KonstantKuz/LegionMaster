using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LegionMaster.Migration.PlayerProgress;
using LegionMaster.Repository;
using SuperMaxim.Core.Extensions;

namespace LegionMaster.Migration
{
    public class LocalRepositoryMigrationService
    {
        private List<RepositoryMigrationEntity> _entities = new List<RepositoryMigrationEntity>();

        public LocalRepositoryMigrationService()
        {
            Register(typeof(PlayerProgressRepository), 3, PlayerProgressMigration.TryMigrationToV3);
            Migrate();
        }

        private void Migrate()
        {
            _entities.ForEach(it => it.Migrate());
        }

        private void Register(Type repository, int version, Action method)
        {
            var entity = FindEntity(repository);
            if (entity == null) {
                entity = new RepositoryMigrationEntity(repository);
                _entities.Add(entity);
            }
            entity.AddMethod(version, method);
        }

        [CanBeNull]
        private RepositoryMigrationEntity FindEntity(Type repository) => _entities.FirstOrDefault(it => it.Repository == repository);

        private class RepositoryMigrationEntity
        {
            private Dictionary<int, Action> _methods = new Dictionary<int, Action>();
            public Type Repository { get; }

            public RepositoryMigrationEntity(Type repository)
            {
                Repository = repository;
            }

            public void AddMethod(int version, Action method)
            {
                if (_methods.ContainsKey(version)) {
                    throw new ArgumentException("Method with version=" + version + " already added for class=" + Repository.Name);
                }
                _methods[version] = method;
            }

            public void Migrate()
            {
                _methods.OrderBy(it => it.Key).ForEach(it => it.Value.Invoke());
            }
        }
    }
}