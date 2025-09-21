using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Entities
{
    public class EntityFactory : IEntityFactory
    {
        private readonly IObjectResolver _resolver;

        [Inject]
        public EntityFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public Entity CreateEntity(Entity prefab)
        {
            var entity = GameObject.Instantiate(prefab);
            
            _resolver.InjectGameObject(entity.gameObject);
            return entity;
        }
    }
}