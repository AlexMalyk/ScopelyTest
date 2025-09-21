using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Battle.Health
{
    public interface IHealthEffect : IEffect
    {
        float ModifyDamage(float incomingDamage);
    }
}