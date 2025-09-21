using System;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Movement
{
    [Serializable]
    public class ProjectileMovementComponent : MovementComponent
    {
        [Header("Projectile Movement")]
        [SerializeField] private bool _leadTarget = false; // Predict target movement
        [SerializeField] private float _predictionTime = 0.1f; // How far ahead to predict
        
        private Vector3 _lastTargetPosition;
        private Vector3 _targetVelocity;
        
        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            _lastTargetPosition = _targetPosition;
            _targetVelocity = Vector3.zero;
        }
        
        protected override void CalculateNextPosition()
        {
            var currentPosition = _entity.CachedTransform.position;
            
            // Calculate predicted position if leading target
            var aimPosition = _targetPosition;
            
            if (_leadTarget && _targetTransform != null)
            {
                // Calculate target velocity
                _targetVelocity = (_targetPosition - _lastTargetPosition) / Time.deltaTime;
                _lastTargetPosition = _targetPosition;
                
                // Predict where target will be
                float timeToTarget = Vector3.Distance(currentPosition, _targetPosition) / CurrentSpeed;
                float predictionFactor = Mathf.Min(timeToTarget, _predictionTime);
                aimPosition = _targetPosition + (_targetVelocity * predictionFactor);
            }
            
            // Calculate direction and move
            var directionToTarget = (aimPosition - currentPosition).normalized;
            _distanceToTarget = Vector3.Distance(currentPosition, aimPosition);
            
            var moveDistance = CurrentSpeed * Time.deltaTime;
            var remainingDistance = _distanceToTarget;
            
            var actualMoveDistance = Mathf.Min(moveDistance, remainingDistance);
            _nextPosition = currentPosition + directionToTarget * actualMoveDistance;
        }
        
        protected override void LookAtTarget()
        {
            // Projectiles always face their movement direction
            if (_hasTarget && _nextPosition != _entity.CachedTransform.position)
            {
                var moveDirection = (_nextPosition - _entity.CachedTransform.position).normalized;
                if (moveDirection != Vector3.zero)
                {
                    _entity.View.rotation = Quaternion.LookRotation(moveDirection);
                }
            }
        }
    }
}