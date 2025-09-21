using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Core.Pooling
{
    public class EntityPoolSystem : IDisposable
    {
        private readonly IEntityFactory _factory;
        
        private Dictionary<Guid, EntityPool> _pools = new();
        
        [Inject]
        public EntityPoolSystem(IEntityFactory factory)
        {
            _factory = factory;
        }

        public Entity Get(IdentifiableConfig config)
        {
            if (config == null) return null;

            var pool = GetOrCreatePool(config);
            return pool.Get();
        }

        public T GetEntity<T>(IdentifiableConfig config) where T : Entity
        {
            var go = Get(config);
            return go?.GetComponent<T>();
        }

        public void Return(Entity entity)
        {
            if (entity == null) return;

            foreach (var pool in _pools.Values)
            {
                if (pool.Return(entity))
                    return;
            }
        }
        
        public void Prewarm(IdentifiableConfig config, int count)
        {
            if (config == null || count <= 0) return;

            var pool = GetOrCreatePool(config);
            pool.Prewarm(count);
            
            Debug.Log($"[EntityPoolSystem] Prewarmed {count} instances of {config.DisplayName}");
        }

        private EntityPool GetOrCreatePool(IdentifiableConfig config)
        {
            var id = config.Id;

            if (!_pools.TryGetValue(id, out var pool))
            {
                pool = new EntityPool(config.Prefab, _factory);
                _pools[id] = pool;
            }

            return pool;
        }

        void IDisposable.Dispose()
        {
            foreach (var pool in _pools.Values)
                pool.Clear();        
        }
    }
}