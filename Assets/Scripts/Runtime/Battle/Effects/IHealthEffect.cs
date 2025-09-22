using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Battle.Effects
{
    public interface IHealthEffect : IEffect
    {
        float ModifyDamage(float incomingDamage);
    }
}