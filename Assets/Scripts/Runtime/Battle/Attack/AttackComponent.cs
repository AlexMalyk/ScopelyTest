using System;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Projectiles;
using TowerDefence.Runtime.Battle.Targeting;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Attack
{
    [Serializable]
    public class AttackComponent : EntityComponent
    {
        [Inject] private ProjectileSpawner _projectileSpawner;
        [Inject] private AttackSystem _attackSystem;

        [Header("Attack Settings")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackRange = 8f;
        
        [Header("Projectile Settings")]
        [SerializeField] private ProjectileConfig _projectileConfig;
        [SerializeField] private Transform _firePoint;

        private TargetingComponent _targetingComponent;
        private float _lastAttackTime;
        private bool _isAttacking;

        public float Damage => _damage;
        public float AttackCooldown => _attackCooldown;
        public float AttackRange => _attackRange;
        public bool CanAttack => Time.time - _lastAttackTime >= _attackCooldown;
        public bool IsAttacking => _isAttacking;

        public event Action<ITargetable> OnAttackStarted;
        public event Action<ITargetable> OnAttackCompleted;
        public event Action<Entity> OnDamageDealt;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _targetingComponent = entity.GetCoreEntityComponent<TargetingComponent>();
            if (_targetingComponent == null)
            {
                Debug.LogWarning($"AttackComponent on {entity.name} requires TargetingComponent");
                return;
            }

            // Set targeting range to match attack range
            _targetingComponent.SetTargetingRange(_attackRange);
            
            // Use fire point transform if available, otherwise use entity view
            if (_firePoint == null)
                _firePoint = entity.View != null ? entity.View : entity.CachedTransform;
            
            _attackSystem?.RegisterAttacker(this);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            _attackSystem?.UnregisterAttacker(this);
        }

        public override void Reset()
        {
            base.Reset();
            _lastAttackTime = 0f;
            _isAttacking = false;
        }

        public void UpdateAttack()
        {
            if (!CanAttack || _targetingComponent == null)
                return;

            if (!_targetingComponent.HasValidTarget)
                return;

            var target = _targetingComponent.CurrentTarget;
            float distanceToTarget = _targetingComponent.GetDistanceToCurrentTarget();

            // Check if target is in attack range
            if (distanceToTarget <= _attackRange)
            {
                Attack(target);
            }
        }

        public void Attack(ITargetable target)
        {
            if (!CanAttack || target == null || !target.IsValidTarget)
                return;

            _lastAttackTime = Time.time;
            _isAttacking = true;

            OnAttackStarted?.Invoke(target);

            AttackWithProjectile(target);

            OnAttackCompleted?.Invoke(target);
            _isAttacking = false;
        }

        public void SetProjectileConfig(ProjectileConfig config)
        {
            _projectileConfig = config;
        }

        private void AttackWithProjectile(ITargetable target)
        {
            if (_projectileConfig == null)
            {
                Debug.LogWarning($"ProjectileConfig is null on {_entity.name}. Cannot attack with projectile.");
                return;
            }

            if (_projectileSpawner == null)
            {
                Debug.LogWarning($"ProjectileSpawner is null on {_entity.name}. Cannot spawn projectile.");
                return;
            }

            var spawnPosition = _firePoint.position;
            _projectileSpawner.SpawnProjectile(_projectileConfig, spawnPosition, target.Entity, _damage, OnProjectileHit);
        }

        private void OnProjectileHit(Entity hitEntity)
        {
            OnDamageDealt?.Invoke(hitEntity);
        }
    }
}