using TowerDefence.Runtime.Core.Modifiers;

namespace TowerDefence.Runtime.Battle.Movement
{
    public interface IMovementModifier : IModifier
    {
        float ModifyMovement(float movementSpeed);
    }
}
