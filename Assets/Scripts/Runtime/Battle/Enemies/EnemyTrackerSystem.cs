using System;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Economy;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Enemies
{
    public class EnemyTrackerSystem
    {
        private readonly IdentifiableConfigProvider<EnemyConfig>  _enemyConfigProvider;
        private readonly GoldSystem _goldSystem;
        private readonly EntitySpawner _spawner;
        
        private int _activeEnemies = 0;
        
        public int ActiveEnemies => _activeEnemies;

        public event Action OnAllEnemiesEliminated;

        [Inject]
        private EnemyTrackerSystem(IdentifiableConfigProvider<EnemyConfig> enemyConfigProvider,
            EntitySpawner spawner, GoldSystem goldSystem)
        {
            _enemyConfigProvider = enemyConfigProvider;
            _spawner = spawner;
            _goldSystem = goldSystem;
        }
        
        public void TrackEnemy(Entity enemy)
        {
            _activeEnemies++;

            var healthComponent = enemy.GetCoreEntityComponent<HealthComponent>();

            healthComponent.OnDeath += HandleEnemyDeath;
            healthComponent.OnEliminated += HandleEnemyElimination;
        }

        private bool IsAnyEnemyActive()
        {
            return _activeEnemies > 0;
        }

        private void HandleEnemyElimination(Entity enemy, HealthComponent healthComponent)
        {
            _activeEnemies = Mathf.Max(0, _activeEnemies - 1);

            healthComponent.OnDeath -= HandleEnemyDeath;
            healthComponent.OnEliminated -= HandleEnemyDeath;
            
            _spawner.Despawn(enemy);
            
            if(!IsAnyEnemyActive())
                OnAllEnemiesEliminated?.Invoke();
        }

        private void HandleEnemyDeath(Entity enemy, HealthComponent healthComponent)
        {
            var configComponent = enemy.GetCoreEntityComponent<ConfigComponent>();
            var config = _enemyConfigProvider.GetByGuid(configComponent.Id);
            _goldSystem.AddGold(config.Reward);
            
            HandleEnemyElimination(enemy, healthComponent);
            
            Debug.Log($"Enemy defeated. Active enemies: {_activeEnemies}");
        }
    }
}