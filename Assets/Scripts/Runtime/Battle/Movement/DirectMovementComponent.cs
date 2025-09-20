using System;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Movement
{
    [Serializable]
    public class DirectMovementComponent : MovementComponent
    {
        protected override void CalculateNextPosition()
        {
            var currentPosition = _entity.transform.position;
            var targetPos = _targetTransform.position;
            var directionToTarget = (targetPos - currentPosition).normalized;
            _distanceToTarget = Vector3.Distance(currentPosition, targetPos);
            
            var moveDistance = CurrentSpeed * Time.deltaTime;
            var remainingDistance = _distanceToTarget - _stoppingDistance;

            var actualMoveDistance = Mathf.Min(moveDistance, remainingDistance);
            _nextPosition = currentPosition + directionToTarget * actualMoveDistance;
        }
    }
}