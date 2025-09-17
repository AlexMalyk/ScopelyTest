using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private Button _playButton;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuEntryPoint>();
            builder.RegisterInstance(_playButton);
        }
    }
}