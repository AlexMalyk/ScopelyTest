using UnityEngine;

namespace TowerDefence.Runtime.Movement
{
    public abstract class MovementComponent : MonoBehaviour
    {
        [SerializeField] protected float _moveSpeed = 2f;
        [SerializeField] protected float _stoppingDistance = 1f;

        protected bool _isReachedTarget;
        protected Transform _targetTransform;
        protected Transform _selfTransform;
        protected Vector3 _targetPosition;
        protected Vector3 _nextPosition;
        protected float _distanceToTarget;
        
        public bool IsReachedTarget => _isReachedTarget;
        
        protected virtual void Awake()
        {
            _selfTransform = transform;
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