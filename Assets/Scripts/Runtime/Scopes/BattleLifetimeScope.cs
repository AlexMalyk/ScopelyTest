using TowerDefence.Runtime.Battle.Enemy;
using TowerDefence.Runtime.Battle.Turrets;
using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyConfig[] _enemyConfigs;
        [SerializeField] private TurretConfig[] _turretConfigs;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemyConfigs).As<EnemyConfig[]>();
            builder.RegisterInstance(_turretConfigs).As<TurretConfig[]>();

            builder.Register<IdentifiableConfigProvider<EnemyConfig>>(Lifetime.Scoped);
            builder.Register<IdentifiableConfigProvider<TurretConfig>>(Lifetime.Scoped);
            
            builder.Register<EntityFactory>(Lifetime.Scoped).AsImplementedInterfaces();
        }
    }
}