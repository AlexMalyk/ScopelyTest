using System;
using TowerDefence.Core.Effects;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Effects
{
    [Serializable]
    public class SlowMovementEffectDefinition : IEffectDefinition
    {
        [SerializeField] private float _slowMultiplier = 0.5f;
        [SerializeField] private float _duration = 3f;

        public EntityComponent CreateEffect()
        {
            return new SlowMovementEffect(_slowMultiplier, _duration);
        }
    }

    [Serializable]
    public class SlowMovementEffect : EntityComponent, IMovementEffect
    {
        [SerializeField] private float _slowMultiplier = 0.5f;
        [SerializeField] private float _duration = 3f;
        
        private float _timeRemaining;

        public SlowMovementEffect(float slowMultiplier, float duration)
        {
            _slowMultiplier = slowMultiplier;
            _duration = duration;
        }

        float IMovementEffect.ModifyMovement(float movementSpeed)
        {
            return movementSpeed * _slowMultiplier;
        }

        protected virtual void Start()
        {
            _timeRemaining = _duration;
        }

        protected virtual void Update()
        {
            _timeRemaining -= Time.deltaTime;
            
            if (_timeRemaining <= 0) 
                _entity.TryRemoveEffect(this);
        }
    }
}
