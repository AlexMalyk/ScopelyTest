using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}