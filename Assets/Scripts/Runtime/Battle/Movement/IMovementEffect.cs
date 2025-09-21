using TowerDefence.Runtime.Core.Effects;

namespace TowerDefence.Runtime.Battle.Movement
{
    public interface IMovementEffect : IEffect
    {
        float ModifyMovement(float movementSpeed);
    }
}
