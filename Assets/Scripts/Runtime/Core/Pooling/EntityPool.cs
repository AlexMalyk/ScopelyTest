using System.Collections.Generic;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Core.Pooling
{
    public class EntityPool
    {
        private readonly IEntityFactory _factory;
        private readonly Entity _prefab;
        private readonly Queue<Entity> _available = new();
        private readonly HashSet<Entity> _active = new();

        public EntityPool(GameObject prefab, IEntityFactory factory)
        {
            _prefab = prefab.GetComponent<Entity>();
            _factory = factory;
        }

        public Entity Get()
        {
            var entity = _available.Count > 0 ? _available.Dequeue() : CreateInstance();
            _active.Add(entity);

            entity.ResetEntity();
            
            return entity;
        }

        public bool Return(Entity entity)
        {
            if (!_active.Contains(entity))
                return false;

            _active.Remove(entity);

            entity.CleanupEntity();
            
            _available.Enqueue(entity);

            return true;
        }
        
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var instance = CreateInstance();
                _available.Enqueue(instance);
            }
        }

        public void Clear()
        {
            foreach (var obj in _active)
                if (obj)
                    Object.Destroy(obj);

            while (_available.Count > 0)
            {
                var obj = _available.Dequeue();
                if (obj) Object.Destroy(obj);
            }
        }

        private Entity CreateInstance()
        {
            var instance = _factory.CreateEntity(_prefab);
            
            instance.gameObject.SetActive(false);

            return instance;
        }
    }
}
