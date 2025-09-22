using TowerDefence.Runtime.Core.Entities;
using VContainer;

namespace TowerDefence.Runtime.Battle.Buildings.PlayerBase
{
    public class PlayerBaseProvider
    {
        private readonly Entity _playerBaseEntity;
        private readonly PlayerBaseComponent _playerBase;

        public PlayerBaseComponent PlayerBase => _playerBase;
        public Entity PlayerBaseEntity => _playerBaseEntity;

        [Inject]
        public PlayerBaseProvider(Entity playerBaseEntity)
        {
            _playerBaseEntity = playerBaseEntity;
            _playerBase = _playerBaseEntity.GetCoreEntityComponent<PlayerBaseComponent>();
        }
    }
}