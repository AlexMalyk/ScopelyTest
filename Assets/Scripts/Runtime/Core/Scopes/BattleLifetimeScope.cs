using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Enemy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyData[] _enemiesDatas;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
            
            builder.RegisterInstance(_enemiesDatas).As<EnemyData[]>();

            builder.Register<IdentifiableConfigProvider<EnemyData>>(Lifetime.Scoped);
        }
    }
}