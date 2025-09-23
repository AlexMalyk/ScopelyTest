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
        
        [Header("Projectile Settings")]
        [SerializeField] private ProjectileConfig _projectileConfig;
        [SerializeField] private Transform _firePoint;

        private TargetingComponent _targetingComponent;
        private float _lastAttackTime;
        private bool _isAttacking;
        
        public bool CanAttack => Time.time - _lastAttackTime >= _attackCooldown;
        public event Action<ITargetable> OnAttackStarted;
        public event Action<ITargetable> OnAttackCompleted;
        public event Action<Entity> OnDamageDealt;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _targetingComponent = entity.GetCoreEntityComponent<TargetingComponent>();
            if (_targetingComponent == null)
            {
                Debug.LogError($"AttackComponent on {entity.name} requires TargetingComponent");
                return;
            }
            
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
            if (!CanAttack)
                return;

            if (!_targetingComponent.HasValidTarget)
                return;

            var target = _targetingComponent.CurrentTarget;
            var distanceToTarget = _targetingComponent.GetDistanceToCurrentTarget();

            if (distanceToTarget <= _targetingComponent.TargetingRange) 
                Attack(target);
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
            _projectileSpawner.SpawnProjectile(_projectileConfig, _firePoint.position, 
                target.Entity, _damage, OnProjectileHit);
        }

        private void OnProjectileHit(Entity hitEntity)
        {
            OnDamageDealt?.Invoke(hitEntity);
        }
    }
}