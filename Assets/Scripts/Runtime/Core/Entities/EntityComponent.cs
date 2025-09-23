using System;
using UnityEngine;

namespace TowerDefence.Runtime.Core.Entities
{
    [Serializable]
    public abstract class EntityComponent
    {
        protected Entity _entity;
        
        public Entity Entity => _entity;

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
        }
        
        public virtual void Reset() { }
        public virtual void Cleanup() { }
    }
}