using System;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Buildings
{
    [Serializable]
    public class PlayerBaseComponent : EntityComponent
    {
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private float _collisionRadius = 0.5f;
        
        private Collider[] _colliders;
        private Vector3 _position;
        
        public event Action<Entity> OnEnemyReached;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _colliders = new Collider[(int)entity.GetCoreEntityComponent<HealthComponent>().MaxHealth];
            _position = _entity.CachedTransform.position;
        }
        
        public void CheckCollision()
        {
            var size = Physics.OverlapSphereNonAlloc(_position, _collisionRadius, _colliders, _targetLayer);

            for (var i = 0; i < size; i++)
            {
                var entity = _colliders[i].GetComponent<Entity>();
                OnEnemyReached?.Invoke(entity);
            }
        }
    }
}