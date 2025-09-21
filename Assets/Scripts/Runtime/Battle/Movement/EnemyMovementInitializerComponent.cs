using System;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Movement
{
    [Serializable]
    public class EnemyMovementInitializer : EntityComponent
    {
        [Inject] private PlayerBaseProvider _playerBaseProvider;
        
        [SerializeField] private bool _targetPlayerBaseOnSpawn = true;
        
        private MovementComponent _movementComponent;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            // Get the movement component from the same entity
            _movementComponent = entity.GetEntityComponent<MovementComponent>();
            
            if (_movementComponent == null)
            {
                Debug.LogWarning($"EnemyMovementInitializer: No MovementComponent found on {entity.name}");
                return;
            }
            
            // Set target to player base if configured
            if (_targetPlayerBaseOnSpawn && _playerBaseProvider?.PlayerBase != null)
            {
                _movementComponent.SetTarget(_playerBaseProvider.PlayerBase.CachedTransform);
            }
        }
        
        public override void Reset()
        {
            base.Reset();
            
            // Re-set the target when entity is reused from pool
            if (_targetPlayerBaseOnSpawn && _playerBaseProvider?.PlayerBase != null && _movementComponent != null)
            {
                _movementComponent.SetTarget(_playerBaseProvider.PlayerBase.CachedTransform);
            }
        }
    }
}