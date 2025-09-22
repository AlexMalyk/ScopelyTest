using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [Serializable]
    public class TargetingComponent : EntityComponent
    {
        [Header("Targeting Settings")]
        [SerializeField] private float _targetingRange = 10f;
        [SerializeReference, SubclassSelector] private ITargetingStrategy _strategy;
        [SerializeField] private LayerMask _targetLayers = -1;
        [SerializeField] private bool _targetEnemies = true;
        [SerializeField] private float _retargetInterval = 0.1f;

        private ITargetable _currentTarget;
        private float _lastRetargetTime;
        private Collider[] _colliderBuffer;
        private List<ITargetable> _potentialTargets;

        public ITargetable CurrentTarget => _currentTarget;
        public float TargetingRange => _targetingRange;
        public bool HasValidTarget => _currentTarget != null && _currentTarget.IsValidTarget;

        public event Action<ITargetable> OnTargetAcquired;
        public event Action<ITargetable> OnTargetLost;

        private const int MaxTargets = 50; // Buffer size for overlap detection

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _colliderBuffer = new Collider[MaxTargets];
            _potentialTargets = new List<ITargetable>(MaxTargets);
            _lastRetargetTime = 0f;
        }

        public override void Reset()
        {
            base.Reset();
            
            ClearTarget();
            _lastRetargetTime = 0f;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            
            ClearTarget();
        }

        public void UpdateTargeting()
        {
            if (_currentTarget is { IsValidTarget: false }) 
                ClearTarget();

            if (Time.time - _lastRetargetTime >= _retargetInterval)
            {
                FindTarget();
                _lastRetargetTime = Time.time;
            }
        }

        public void ForceRetarget()
        {
            FindTarget();
            _lastRetargetTime = Time.time;
        }

        public float GetDistanceToCurrentTarget()
        {
            if (!HasValidTarget)
                return float.MaxValue;
                
            return Vector3.Distance(_entity.CachedTransform.position, _currentTarget.TargetTransform.position);
        }

        private void FindTarget()
        {
            _potentialTargets.Clear();
            
            var position = _entity.CachedTransform.position;
            var targetsFound = Physics.OverlapSphereNonAlloc(position, _targetingRange, _colliderBuffer, _targetLayers);

            for (int i = 0; i < targetsFound; i++)
            {
                var entity = _colliderBuffer[i].GetComponent<Entity>();
                var targetable = entity.GetCoreEntityComponent<TargetableComponent>();
                if (IsValidTarget(targetable)) 
                    _potentialTargets.Add(targetable);
            }

            var bestTarget = SelectBestTarget(_potentialTargets);
            
            if (bestTarget != _currentTarget) 
                SetTarget(bestTarget);
        }

        private bool IsValidTarget(ITargetable target)
        {
            if (target == null || !target.IsValidTarget)
                return false;

            // Check if target matches what we're looking for (enemy vs friendly)
            if (target.IsEnemy != _targetEnemies)
                return false;

            // Don't target self
            if (target.Entity == _entity)
                return false;

            return true;
        }

        private ITargetable SelectBestTarget(List<ITargetable> targets)
        {
            return _strategy.SelectBestTarget(targets, _entity.CachedTransform.position);
        }

        private void SetTarget(ITargetable target)
        {
            var previousTarget = _currentTarget;
            _currentTarget = target;

            if (previousTarget != target)
            {
                if (previousTarget != null) 
                    OnTargetLost?.Invoke(previousTarget);

                if (_currentTarget != null) 
                    OnTargetAcquired?.Invoke(_currentTarget);
            }
        }

        private void ClearTarget()
        {
            if (_currentTarget == null) return;
            
            var lostTarget = _currentTarget;
            _currentTarget = null;
            OnTargetLost?.Invoke(lostTarget);
        }
    }
}