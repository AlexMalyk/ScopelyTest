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
        private HealthComponent  _healthComponent;
        private Vector3 _position;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _healthComponent = entity.GetCoreEntityComponent<HealthComponent>();
            _colliders = new Collider[(int)_healthComponent.MaxHealth];
            _position = _entity.CachedTransform.position;
        }
        
        public void CheckCollision()
        {
            var size = Physics.OverlapSphereNonAlloc(_position, _collisionRadius, _colliders, _targetLayer);

            for (var i = 0; i < size; i++) 
                OnEnemyReachedBase();
        }
        
        private void OnEnemyReachedBase()
        {
            _healthComponent.TakeDamage(1);
        }
    }
}