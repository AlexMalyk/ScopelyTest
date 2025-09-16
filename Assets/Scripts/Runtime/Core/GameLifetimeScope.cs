using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}