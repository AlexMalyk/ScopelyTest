using System.Collections.Generic;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Core.Modifiers;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Movement
{
    public abstract class MovementComponent : EntityComponent, IEntityComponentListener, IModifierRegisterer<IMovementModifier>
    {
        [SerializeField] protected float _baseSpeed = 2f;
        [SerializeField] protected float _stoppingDistance = 1f;

        protected bool _isReachedTarget;
        protected Transform _targetTransform;
        protected Transform _selfTransform;
        protected Vector3 _targetPosition;
        protected Vector3 _nextPosition;
        protected float _distanceToTarget;
        
        private List<IMovementModifier> movementModifiers = new();

        public bool IsReachedTarget => _isReachedTarget;
        public float BaseSpeed => _baseSpeed;
        public float CurrentSpeed => CalculateModifiedSpeed();
        
        protected virtual void Awake()
        {
            _selfTransform = transform;
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            entity.OnEntityComponentAdded += OnEntityComponentAdded;
            entity.OnEntityComponentRemoving += OnEntityComponentRemoving;
        }

        public void OnEntityComponentAdded(EntityComponent component)
        {
            if (component is IMovementModifier movementModifier) 
                RegisterModifier(movementModifier);
        }

        public void OnEntityComponentRemoving(EntityComponent component)
        {
            if (component is IMovementModifier movementModifier) 
                UnregisterModifier(movementModifier);
        }

        public void RegisterModifier(IMovementModifier modifier)
        {
            if (!movementModifiers.Contains(modifier)) 
                movementModifiers.Add(modifier);
        }

        public void UnregisterModifier(IMovementModifier modifier)
        {
            movementModifiers.Remove(modifier);
        }

        private float CalculateModifiedSpeed()
        {
            var modifiedSpeed = _baseSpeed;

            foreach (var modifier in movementModifiers) 
                modifiedSpeed = modifier.ModifyMovement(modifiedSpeed);

            return modifiedSpeed;
        }
        
        public virtual void Initialize(Transform target)
        {
            _isReachedTarget = false;
            _targetTransform = target;
            _targetPosition = _targetTransform.position;
            CalculateNextPosition();
            LookAtTarget();

            OnInitialize();
        }
        
        protected virtual void OnInitialize() { }
        
        [ContextMenu("Move")]
        public virtual void Move()
        {
            if(_isReachedTarget) return;
            
            MoveToNextPosition();
            CheckTargetReached();
            LookAtTarget();
            CalculateNextPosition();
        }
        
        protected abstract void CalculateNextPosition();

        protected virtual void MoveToNextPosition()
        {
            _selfTransform.position = _nextPosition;
        }

        protected virtual void CheckTargetReached()
        {
            _isReachedTarget = Vector3.Distance(_nextPosition, _targetPosition) <= _stoppingDistance;
        }

        protected virtual void LookAtTarget()
        {
            _selfTransform.LookAt(_targetPosition);
        }
    }
}