using TowerDefence.Runtime.Battle;
using TowerDefence.Runtime.Battle.Attack;
using TowerDefence.Runtime.Battle.Buildings.PlayerBase;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Economy;
using TowerDefence.Runtime.Battle.Enemies;
using TowerDefence.Runtime.Battle.Movement;
using TowerDefence.Runtime.Battle.Placement;
using TowerDefence.Runtime.Battle.Projectiles;
using TowerDefence.Runtime.Battle.UI;
using TowerDefence.Runtime.Battle.Waving;
using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Core.Pooling;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameObject _winPopup;
        [SerializeField] private GameObject _losePopup;
        [SerializeField] private Entity _playerBaseEntity;
        [SerializeField] private EnemyConfig[] _enemyConfigs;
        [SerializeField] private PlaceableConfig[] _placeableConfigs;
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private WaveConfig[] _waveConfigs;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemyConfigs).As<EnemyConfig[]>();
            builder.RegisterInstance(_placeableConfigs).As<PlaceableConfig[]>();
            builder.RegisterInstance(_spawnPoints).As<SpawnPoint[]>();
            builder.RegisterInstance(_waveConfigs).As<WaveConfig[]>();

            builder.Register<IdentifiableConfigProvider<EnemyConfig>>(Lifetime.Scoped);
            builder.Register<IdentifiableConfigProvider<PlaceableConfig>>(Lifetime.Scoped);

            builder.Register<PlayerBaseHealthSystem>(Lifetime.Scoped);
            builder.Register<PlayerBaseProvider>(Lifetime.Scoped).WithParameter(_playerBaseEntity).AsSelf();
            
            builder.Register<SpawnPointsProvider>(Lifetime.Scoped);

            builder.Register<EntityFactory>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<EntitySpawner>(Lifetime.Scoped);
            builder.Register<EntityPoolSystem>(Lifetime.Scoped);
            
            builder.Register<WaveSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<EnemyTrackerSystem>(Lifetime.Scoped);
            
            builder.Register<MovementSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            
            builder.Register<ProjectileSpawner>(Lifetime.Scoped);
            builder.Register<ProjectileSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            
            builder.Register<AttackSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.Register<GoldSystem>(Lifetime.Scoped);
            
            builder.Register<BasicPlacementValidator>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<PlacementSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();

            builder.RegisterComponentInHierarchy<PlaceableSelectionUI>();
            
            builder.Register<BattleLoopSystem>(Lifetime.Scoped)
                .WithParameter(_winPopup)
                .WithParameter(_losePopup)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}