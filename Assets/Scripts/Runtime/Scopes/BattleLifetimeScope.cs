using TowerDefence.Runtime.Battle.Enemy;
using TowerDefence.Runtime.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyConfig[] _enemyConfigs;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemyConfigs).As<EnemyConfig[]>();

            builder.Register<IdentifiableConfigProvider<EnemyConfig>>(Lifetime.Scoped);
        }
    }
}