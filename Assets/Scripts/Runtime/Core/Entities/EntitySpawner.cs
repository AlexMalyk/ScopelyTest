using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Pooling;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Core.Entities
{
    public class EntitySpawner
    {
        private readonly EntityPoolSystem _entityPoolSystem;

        [Inject]
        public EntitySpawner(EntityPoolSystem entityPoolSystem)
        {
            _entityPoolSystem = entityPoolSystem;
        }

        public Entity Spawn(IdentifiableConfig config, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var entity = _entityPoolSystem.GetEntity<Entity>(config);
            if (entity == null)
                return null;

            entity.transform.position = position;
            entity.transform.rotation = rotation;
            
            if (parent != null)
                entity.transform.SetParent(parent);

            entity.OnSpawn();

            return entity;
        }

        public Entity Spawn(IdentifiableConfig config, Transform parent)
        {
            return Spawn(config, Vector3.zero, Quaternion.identity, parent);
        }

        public void Despawn(Entity entity)
        {
            if (entity == null)
                return;
            
            entity.OnDespawn();

            _entityPoolSystem.Return(entity);
        }
    }
}