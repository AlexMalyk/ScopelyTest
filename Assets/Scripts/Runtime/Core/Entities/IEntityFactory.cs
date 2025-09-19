using UnityEngine;

namespace TowerDefence.Runtime.Core.Entities
{
    public interface IEntityFactory
    {
        Entity CreateEntity(Entity prefab, Vector3 position = default, 
            Quaternion rotation = default, Transform parent = null);

        Entity CreateEntity(Entity prefab, Transform parent);
    }
}