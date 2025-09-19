using TowerDefence.Runtime.Core.Modifiers;

namespace TowerDefence.Runtime.Battle.Health
{
    public interface IHealthModifier : IModifier
    {
        float ModifyDamage(float incomingDamage);
    }
}