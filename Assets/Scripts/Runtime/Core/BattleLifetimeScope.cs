using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core
{
    public class BattleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleEntryPoint>();
        }
    }
}