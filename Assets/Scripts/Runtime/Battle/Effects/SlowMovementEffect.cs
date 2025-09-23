using System;
using TowerDefence.Core.Effects;
using TowerDefence.Runtime.Core.Effects;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

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
        [Inject] private EffectsSystem _effectsSystem;
        
        [SerializeField] private float _slowMultiplier = 0.5f;
        [SerializeField] private float _duration = 3f;
        
        private float _timeRemaining;

        public SlowMovementEffect(float slowMultiplier, float duration)
        {
            _slowMultiplier = slowMultiplier;
            _duration = duration;
            
            _timeRemaining = duration;
        }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            
            _effectsSystem.RegisterEffect(this);
        }

        float IMovementEffect.ModifyMovement(float movementSpeed)
        {
            return movementSpeed * _slowMultiplier;
        }

        public bool TryRemoveEffect()
        {
            if(_entity != null)
                return _entity.TryRemoveEffect(this);

            return false;
        }

        void IEffect.UpdateEffect(float deltaTime)
        {
            _timeRemaining -= deltaTime;
            
            if (IsExpired())
            {
                TryRemoveEffect();
            }
        }

        public bool IsExpired()
        {
            return _timeRemaining <= 0;
        }
    }
}
