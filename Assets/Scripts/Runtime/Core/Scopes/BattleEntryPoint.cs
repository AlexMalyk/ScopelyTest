using TowerDefence.Runtime.Battle.Enemy;
using TowerDefence.Runtime.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class BattleEntryPoint : IStartable
    {
        [Inject] private IdentifiableConfigProvider<EnemyConfig>  configProvider;
        
        void IStartable.Start()
        {
            Debug.Log($"[{nameof(BattleEntryPoint)}] Battle entry point {configProvider.Count}");
        }
    }
}