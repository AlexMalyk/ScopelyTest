using System;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [Serializable]
    public class TargetableComponent : EntityComponent, ITargetable
    {
        [SerializeField] private bool _isEnemy = true;
        [SerializeField] private float _targetPriority = 1f;

        private HealthComponent _healthComponent;

        public Transform TargetTransform => _entity.CachedTransform;
        public bool IsValidTarget => _entity != null && _entity.gameObject.activeInHierarchy && !IsDead;
        public float TargetPriority => _targetPriority;
        public float HealthPercentage => _healthComponent != null ? _healthComponent.CurrentHealth / _healthComponent.MaxHealth : 1f;
        public bool IsEnemy => _isEnemy;

        private bool IsDead => _healthComponent is { CurrentHealth: <= 0 };

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _healthComponent = entity.GetCoreEntityComponent<HealthComponent>();
        }

        public override void Reset()
        {
            base.Reset();
            
            _healthComponent = _entity.GetCoreEntityComponent<HealthComponent>();
        }
    }
}