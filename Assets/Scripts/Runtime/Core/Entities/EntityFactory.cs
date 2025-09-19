using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Entities
{
    public class EntityFactory : IEntityFactory
    {
        private readonly IObjectResolver _container;

        [Inject]
        public EntityFactory(IObjectResolver container)
        {
            _container = container;
        }

        public Entity CreateEntity(Entity prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var instance = Object.Instantiate(prefab, position, rotation, parent);
            _container.InjectGameObject(instance.gameObject);
            return instance;
        }

        public Entity CreateEntity(Entity prefab, Transform parent)
        {
            return CreateEntity(prefab, Vector3.zero, Quaternion.identity, parent);
        }
    }
}