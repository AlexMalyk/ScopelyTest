using TowerDefence.Runtime.Core.Pooling;
using VContainer;

namespace TowerDefence.Runtime.Core.Entities
{
    public class EntityObjectPool<T> : ObjectPool<T> where T : Entity
    {
        private readonly IEntityFactory _entityFactory;

        [Inject]
        public EntityObjectPool(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        protected override T CreateInstance(T prefab)
        {
            return _entityFactory.CreateEntity(prefab) as T;
        }
    }
}
