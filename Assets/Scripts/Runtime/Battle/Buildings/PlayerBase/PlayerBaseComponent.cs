using System;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Buildings
{
    [Serializable]
    public class PlayerBaseComponent : EntityComponent
    {
        private Transform _cachedTransform;
        
        public Transform CachedTransform => _cachedTransform;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _cachedTransform = _entity.View;
        }
    }
}