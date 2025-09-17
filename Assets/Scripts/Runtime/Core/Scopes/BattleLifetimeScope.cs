using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Enemy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyConfig[] _enemiesDatas;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemiesDatas).As<EnemyConfig[]>();

            builder.Register<IdentifiableConfigProvider<EnemyConfig>>(Lifetime.Scoped);
        }
    }
}