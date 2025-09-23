using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Effects;
using TowerDefence.Runtime.Core.Efefcts;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Movement
{
    [Serializable]
    public abstract class MovementComponent : EntityComponent, IEntityComponentListener, IEffectRegisterer<IMovementEffect>
    {
        [Inject] private MovementSystem _movementSystem;
        
        [SerializeField] protected float _baseSpeed = 2f;
        [SerializeField] protected float _stoppingDistance = 1f;

        protected bool _isReachedTarget;
        protected Transform _targetTransform;
        protected Vector3 _targetPosition;
        protected Vector3 _nextPosition;
        protected float _distanceToTarget;
        protected bool _hasTarget;
        
        private List<IMovementEffect> _movementEffects = new();

        public bool IsReachedTarget => _isReachedTarget;
        public bool HasTarget => _hasTarget;
        public float BaseSpeed => _baseSpeed;
        public float CurrentSpeed => CalculateModifiedSpeed();
        public float StoppingDistance => _stoppingDistance;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            entity.OnEntityComponentAdded += OnEntityComponentAdded;
            entity.OnEntityComponentRemoving += OnEntityComponentRemoving;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            
            _movementEffects.Clear();
            _movementSystem.UnregisterComponent(this);
        }

        public override void Reset()
        {
            base.Reset();
            
            _movementSystem.RegisterComponent(this);
            
            _isReachedTarget = false;
            _hasTarget = false;
            _targetTransform = null;
            _targetPosition = Vector3.zero;
            _nextPosition = Vector3.zero;
            _distanceToTarget = 0f;
        }

        public void OnEntityComponentAdded(EntityComponent component)
        {
            if (component is IMovementEffect effect) 
                RegisterEffect(effect);
        }

        public void OnEntityComponentRemoving(EntityComponent component)
        {
            if (component is IMovementEffect effect) 
                UnregisterEffect(effect);
        }

        public void RegisterEffect(IMovementEffect effect)
        {
            if (!_movementEffects.Contains(effect)) 
                _movementEffects.Add(effect);
        }

        public void UnregisterEffect(IMovementEffect effect)
        {
            _movementEffects.Remove(effect);
        }

        protected float CalculateModifiedSpeed()
        {
            var modifiedSpeed = _baseSpeed;

            foreach (var effect in _movementEffects) 
                modifiedSpeed = effect.ModifyMovement(modifiedSpeed);

            return modifiedSpeed;
        }
        
        public virtual void SetTarget(Transform target)
        {
            if (target == null)
            {
                ClearTarget();
                return;
            }
            
            _isReachedTarget = false;
            _hasTarget = true;
            _targetTransform = target;
            _targetPosition = _targetTransform.position;
            LookAtTarget();
        }
        
        public virtual void SetTarget(Vector3 position)
        {
            _isReachedTarget = false;
            _hasTarget = true;
            _targetTransform = null;
            _targetPosition = position;
            LookAtTarget();
        }
        
        public virtual void ClearTarget()
        {
            _hasTarget = false;
            _targetTransform = null;
            _targetPosition = Vector3.zero;
            _isReachedTarget = false;
        }
        
        public void Move()
        {
            if (!_hasTarget || _isReachedTarget) return;
            
            _targetPosition = _targetTransform.position;
            
            CalculateNextPosition();
            MoveToNextPosition();
            CheckTargetReached();
            LookAtTarget();
        }
        
        protected abstract void CalculateNextPosition();

        protected virtual void MoveToNextPosition()
        {
            _entity.CachedTransform.position = _nextPosition;
        }

        protected virtual void CheckTargetReached()
        {
            _isReachedTarget = Vector3.Distance(_nextPosition, _targetPosition) <= _stoppingDistance;
        }

        protected virtual void LookAtTarget()
        {
            if (_hasTarget) 
                _entity.View.LookAt(_targetPosition);
        }
    }
}