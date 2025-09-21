using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Core.Efefcts
{
    public interface IEffectRegisterer<T> where T : IEffect
    {
        void RegisterEffect(T effect);
        void UnregisterEffect(T effect);
    }
}