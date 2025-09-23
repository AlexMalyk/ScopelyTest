using System;
using TowerDefence.Core.Effects;
using TowerDefence.Runtime.Core.Effects;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Effects
{
    [Serializable]
    public class DecreaseDamageEffectDefinition : IEffectDefinition
    {
        [SerializeField] private float _decreaseAmount;
        
        EntityComponent IEffectDefinition.CreateEffect()
        {
            return new DecreaseDamageEffect(_decreaseAmount);
        }
    }

    [Serializable]
    public class DecreaseDamageEffect : EntityComponent, IHealthEffect
    {
        [SerializeField] private float _decreaseAmount;
        
        public DecreaseDamageEffect(float decreaseAmount)
        {
            _decreaseAmount = decreaseAmount;
        }

        public float ModifyDamage(float incomingDamage)
        {
            return Mathf.Max(0f, incomingDamage - _decreaseAmount);
        }

        void IEffect.UpdateEffect(float deltaTime) { }
        public bool IsExpired()
        {
            return false;
        }
    }
}
