using System;
using System.Collections.Generic;
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
        [Inject] private PlayerBaseProvider _playerBaseProvider;
        
        [SerializeField] protected float _baseSpeed = 2f;
        [SerializeField] protected float _stoppingDistance = 1f;

        protected bool _isReachedTarget;
        protected Transform _targetTransform;
        protected Vector3 _targetPosition;
        protected Vector3 _nextPosition;
        protected float _distanceToTarget;
        
        private List<IMovementEffect> movementModifiers = new();

        public bool IsReachedTarget => _isReachedTarget;
        public float BaseSpeed => _baseSpeed;
        public float CurrentSpeed => CalculateModifiedSpeed();

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            entity.OnEntityComponentAdded += OnEntityComponentAdded;
            entity.OnEntityComponentRemoving += OnEntityComponentRemoving;
            
            _movementSystem.RegisterComponent(this);

            InitializeMovement(_playerBaseProvider.PlayerBase.CachedTransform);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            
            _movementSystem.UnregisterComponent(this);
        }

        public void OnEntityComponentAdded(EntityComponent component)
        {
            if (component is IMovementEffect movementModifier) 
                RegisterEffect(movementModifier);
        }

        public void OnEntityComponentRemoving(EntityComponent component)
        {
            if (component is IMovementEffect movementModifier) 
                UnregisterEffect(movementModifier);
        }

        public void RegisterEffect(IMovementEffect effect)
        {
            if (!movementModifiers.Contains(effect)) 
                movementModifiers.Add(effect);
        }

        public void UnregisterEffect(IMovementEffect effect)
        {
            movementModifiers.Remove(effect);
        }

        protected float CalculateModifiedSpeed()
        {
            var modifiedSpeed = _baseSpeed;

            foreach (var modifier in movementModifiers) 
                modifiedSpeed = modifier.ModifyMovement(modifiedSpeed);

            return modifiedSpeed;
        }
        
        protected virtual void InitializeMovement(Transform target)
        {
            _isReachedTarget = false;
            _targetTransform = target;
            _targetPosition = _targetTransform.position;
            LookAtTarget();
        }
        
        public void Move()
        {
            if(_isReachedTarget) return;
            
            CalculateNextPosition();
            MoveToNextPosition();
            CheckTargetReached();
            LookAtTarget();
        }
        
        protected abstract void CalculateNextPosition();

        protected virtual void MoveToNextPosition()
        {
            _entity.transform.position = _nextPosition;
        }

        protected virtual void CheckTargetReached()
        {
            _isReachedTarget = Vector3.Distance(_nextPosition, _targetPosition) <= _stoppingDistance;
        }

        protected virtual void LookAtTarget()
        {
            _entity.transform.LookAt(_targetPosition);
        }
    }
}