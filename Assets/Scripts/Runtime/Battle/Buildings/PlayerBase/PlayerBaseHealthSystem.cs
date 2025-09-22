using System;
using VContainer;

namespace TowerDefence.Runtime.Battle.Buildings.PlayerBase
{
    public class PlayerBaseHealthSystem
    {
        private readonly PlayerBaseProvider _playerBaseProvider;

        private PlayerBaseComponent _playerBaseComponent;
        
        public event Action OnPlayerBaseDestroyed;

        [Inject]
        public PlayerBaseHealthSystem(PlayerBaseProvider playerBaseProvider)
        {
            _playerBaseProvider = playerBaseProvider;
            
            _playerBaseComponent = _playerBaseProvider.PlayerBaseEntity.GetCoreEntityComponent<PlayerBaseComponent>();
        }

        private void DestroyPlayerBase()
        {
            OnPlayerBaseDestroyed?.Invoke();
        }
    }
}
