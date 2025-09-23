using System;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Buildings.PlayerBase
{
    public class PlayerBaseHealthSystem : ITickable, IDisposable
    {
        private readonly PlayerBaseProvider _playerBaseProvider;
        private readonly EntitySpawner _entitySpawner;

        private PlayerBaseComponent _playerBaseComponent;
        private HealthComponent _healthComponent;
        
        public event Action OnPlayerBaseDestroyed;

        [Inject]
        public PlayerBaseHealthSystem(PlayerBaseProvider playerBaseProvider, EntitySpawner entitySpawner)
        {
            _playerBaseProvider = playerBaseProvider;
            _entitySpawner = entitySpawner;
            
            _playerBaseComponent = _playerBaseProvider.PlayerBaseEntity.GetCoreEntityComponent<PlayerBaseComponent>();
            _healthComponent = _playerBaseProvider.PlayerBaseEntity.GetCoreEntityComponent<HealthComponent>();

            _playerBaseComponent.OnEnemyReached += HandleEnemyReachedBase;
            _healthComponent.OnDeath += DestroyPlayerBase;
        }

        private void HandleEnemyReachedBase(Entity entity)
        {
            Debug.Log($"Enemy reached base {entity.name} health {_healthComponent.CurrentHealth - 1}");
            var health = entity.GetCoreEntityComponent<HealthComponent>();
            health.Eliminate();
            _entitySpawner.Despawn(entity);
            
            _healthComponent.TakeDamage(1);
        }

        private void DestroyPlayerBase(Entity entity, HealthComponent health)
        {
            OnPlayerBaseDestroyed?.Invoke();
        }

        void ITickable.Tick()
        {
            _playerBaseComponent.CheckCollision();
        }

        void IDisposable.Dispose()
        {
            _playerBaseComponent.OnEnemyReached -= HandleEnemyReachedBase;
            _healthComponent.OnDeath -= DestroyPlayerBase;
        }
    }
}
