using TowerDefence.Runtime.Battle.Attack;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Movement;
using TowerDefence.Runtime.Battle.Projectiles;
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
        [SerializeField] private Entity _playerBaseEntity;
        [SerializeField] private EnemyConfig[] _enemyConfigs;
        [SerializeField] private TurretConfig[] _turretConfigs;
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private WaveConfig[] _waveConfigs;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemyConfigs).As<EnemyConfig[]>();
            builder.RegisterInstance(_turretConfigs).As<TurretConfig[]>();
            builder.RegisterInstance(_spawnPoints).As<SpawnPoint[]>();
            builder.RegisterInstance(_waveConfigs).As<WaveConfig[]>();

            builder.Register<IdentifiableConfigProvider<EnemyConfig>>(Lifetime.Scoped);
            builder.Register<IdentifiableConfigProvider<TurretConfig>>(Lifetime.Scoped);

            builder.Register<PlayerBaseProvider>(Lifetime.Scoped).WithParameter(_playerBaseEntity).AsSelf();
            
            builder.Register<SpawnPointsProvider>(Lifetime.Scoped);

            builder.Register<EntityFactory>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<EntitySpawner>(Lifetime.Scoped);
            builder.Register<EntityPoolSystem>(Lifetime.Scoped);
            
            builder.Register<WaveSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            
            builder.Register<MovementSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            
            builder.Register<ProjectileSpawner>(Lifetime.Scoped);
            builder.Register<ProjectileSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            
            builder.Register<AttackSystem>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        }
    }
}