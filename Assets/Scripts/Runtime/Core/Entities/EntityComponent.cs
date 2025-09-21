using System;
using UnityEngine;

namespace TowerDefence.Runtime.Core.Entities
{
    [Serializable]
    public abstract class EntityComponent
    {
        protected Entity _entity;

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
            Debug.Log($"{GetType().Name} initialized on {entity.name}", _entity.gameObject);
        }
        
        public virtual void Reset() { }
        public virtual void Cleanup() { }
    }
}