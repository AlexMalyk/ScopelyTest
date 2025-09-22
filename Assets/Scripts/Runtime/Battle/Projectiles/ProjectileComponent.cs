using System;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Battle.Movement;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Projectiles
{
    [Serializable]
    public class ProjectileComponent : EntityComponent
    {
        [Inject] private ProjectileSystem _projectileSystem;
        
        [Header("Projectile Settings")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private float _collisionRadius = 0.5f;
        [SerializeField] private bool _destroyOnReachTarget = true;

        private Collider[] _colliders;
        private MovementComponent _movementComponent;
        private Entity _targetEntity;
        private float _currentLifeTime;
        private bool _isActive;
        private Action<Entity> _onHitCallback;
        private Action<Entity> _onDestroyCallback;
        
        public float Damage => _damage;
        public bool IsActive => _isActive;

        private const int maxColliders = 10;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            _colliders = new Collider[maxColliders];
            
            _movementComponent = entity.GetCoreEntityComponent<MovementComponent>();
            if (_movementComponent == null)
            {
                Debug.LogError($"ProjectileBehaviorComponent requires MovementComponent on {entity.name}");
                return;
            }
            
            _projectileSystem.RegisterProjectile(this);
        }
        
        public override void Cleanup()
        {
            base.Cleanup();
            _projectileSystem.UnregisterProjectile(this);
        }
        
        public override void Reset()
        {
            base.Reset();
            _targetEntity = null;
            _currentLifeTime = 0f;
            _isActive = false;
            _onHitCallback = null;
            _onDestroyCallback = null;
        }
        
        public void Launch(Entity target, float damage, Action<Entity> onHitCallback = null, Action<Entity> onDestroyCallback = null)
        {
            _targetEntity = target;
            _damage = damage;
            _currentLifeTime = 0f;
            _isActive = true;
            _onHitCallback = onHitCallback;
            _onDestroyCallback = onDestroyCallback;
            
            _movementComponent.SetTarget(target.CachedTransform);
        }
        
        public void Launch(Vector3 targetPosition, float damage, Action<Entity> onHitCallback = null, Action<Entity> onDestroyCallback = null)
        {
            _targetEntity = null;
            _damage = damage;
            _currentLifeTime = 0f;
            _isActive = true;
            _onHitCallback = onHitCallback;
            _onDestroyCallback = onDestroyCallback;
            
            _movementComponent.SetTarget(targetPosition);
        }
        
        public void UpdateProjectile(float deltaTime)
        {
            if (!_isActive) return;
            
            _currentLifeTime += deltaTime;
            
            if (_currentLifeTime >= _lifeTime)
            {
                DestroyProjectile();
                return;
            }
            
            if (_movementComponent is { IsReachedTarget: true } && _destroyOnReachTarget)
            {
                OnTargetReached();
                return;
            }
            
            CheckCollision();
        }
        
        private void CheckCollision()
        {
            var currentPosition = _entity.CachedTransform.position;
            var size = Physics.OverlapSphereNonAlloc(currentPosition, _collisionRadius, _colliders, _targetLayer);

            for (var i = 0; i < size; i++)
            {
                var collider = _colliders[i];

                // Check if hit entity has Entity component
                var hitEntity = collider.GetComponent<Entity>();
                if (hitEntity != null)
                {
                    OnHit(hitEntity);
                    return; // Hit something, stop checking
                }
            }
        }
        
        private void OnTargetReached()
        {
            if (_targetEntity != null && _targetEntity.gameObject.activeInHierarchy)
                OnHit(_targetEntity);
            else
                DestroyProjectile();
        }
        
        private void OnHit(Entity hitEntity)
        {
            var healthComponent = hitEntity.GetCoreEntityComponent<HealthComponent>();
            healthComponent?.TakeDamage(_damage);

            _onHitCallback?.Invoke(hitEntity);
            
            DestroyProjectile();
        }
        
        private void DestroyProjectile()
        {
            _isActive = false;

            _movementComponent?.ClearTarget();

            _onDestroyCallback?.Invoke(_entity);
        }
    }
}