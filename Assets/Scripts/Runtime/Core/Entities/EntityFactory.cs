using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Entities
{
    public class EntityFactory : IEntityFactory
    {
        private readonly IObjectResolver _resolver;

        private int _index = 0;
        
        [Inject]
        public EntityFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public Entity CreateEntity(Entity prefab)
        {
            var entity = GameObject.Instantiate(prefab);
            
#if UNITY_EDITOR
            entity.gameObject.name = $"{prefab.name}_{_index}";
#endif
            _index++;
            
            _resolver.InjectGameObject(entity.gameObject);
            return entity;
        }
    }
}