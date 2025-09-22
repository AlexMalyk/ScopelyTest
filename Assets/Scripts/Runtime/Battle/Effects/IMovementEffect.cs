using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Battle.Effects
{
    public interface IMovementEffect : IEffect
    {
        float ModifyMovement(float movementSpeed);
    }
}
