using System;
using TowerDefence.Runtime.Battle.Buildings.PlayerBase;
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
            
            _movementComponent = entity.GetCoreEntityComponent<MovementComponent>();
            
            if (_targetPlayerBaseOnSpawn) 
                _movementComponent.SetTarget(_playerBaseProvider.PlayerBaseEntity.CachedTransform);
        }
        
        public override void Reset()
        {
            base.Reset();
            
            if (_targetPlayerBaseOnSpawn) 
                _movementComponent.SetTarget(_playerBaseProvider.PlayerBaseEntity.CachedTransform);
        }
    }
}