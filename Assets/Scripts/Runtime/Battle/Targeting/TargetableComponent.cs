using System;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [Serializable]
    public class TargetableComponent : EntityComponent, ITargetable
    {
        [Header("Targeting Settings")]
        [SerializeField] private bool _isEnemy = true;
        [SerializeField] private float _targetPriority = 1f;
        [SerializeField] private Transform _targetTransform;

        private HealthComponent _healthComponent;

        public Entity Entity => _entity;
        public Transform TargetTransform => _targetTransform != null ? _targetTransform : _entity.CachedTransform;
        public bool IsValidTarget => _entity != null && _entity.gameObject.activeInHierarchy && !IsDead;
        public float TargetPriority => _targetPriority;
        public float HealthPercentage => _healthComponent != null ? _healthComponent.CurrentHealth / _healthComponent.MaxHealth : 1f;
        public bool IsEnemy => _isEnemy;

        private bool IsDead => _healthComponent is { CurrentHealth: <= 0 };

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _healthComponent = entity.GetEntityComponent<HealthComponent>();
            
            if (_targetTransform == null)
                _targetTransform = entity.View != null ? entity.View : entity.CachedTransform;
        }

        public override void Reset()
        {
            base.Reset();
            
            _healthComponent = _entity.GetEntityComponent<HealthComponent>();
        }

        public void SetTargetPriority(float priority)
        {
            _targetPriority = priority;
        }

        public void SetIsEnemy(bool isEnemy)
        {
            _isEnemy = isEnemy;
        }

        public void SetTargetTransform(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }
    }
}