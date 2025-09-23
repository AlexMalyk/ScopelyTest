using System.Collections.Generic;
using TowerDefence.Runtime.Core.Efefcts;
using TowerDefence.Runtime.Core.Effects;
using UnityEngine;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Effects
{
    public class EffectsSystem : ITickable, ILateTickable, IEffectRegisterer<IEffect>
    {
        private HashSet<IEffect> _effects = new();
        private List<IEffect> _effectsToRemove = new();

        public void RegisterEffect(IEffect effect)
        {
            _effects.Add(effect);
        }

        public void UnregisterEffect(IEffect effect)
        {
            _effects.Remove(effect);
        }

        void ITickable.Tick()
        {
            var deltaTime = Time.deltaTime;

            foreach (var effect in _effects)
            {
                if (effect == null || effect.IsExpired())
                {
                    _effectsToRemove.Add(effect);
                    continue;
                }

                effect.UpdateEffect(deltaTime);
            }
        }

        void ILateTickable.LateTick()
        {
            foreach (var effect in _effectsToRemove)
            {
                effect.TryRemoveEffect();
                _effects.Remove(effect);
            }
            
            _effectsToRemove.Clear();
        }
    }
}