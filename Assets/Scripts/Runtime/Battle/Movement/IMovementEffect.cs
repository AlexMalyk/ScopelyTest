using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Battle.Movement
{
    public interface IMovementModifier : IEffect
    {
        float ModifyMovement(float movementSpeed);
    }
}
