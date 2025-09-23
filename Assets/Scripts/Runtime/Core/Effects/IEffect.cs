namespace TowerDefence.Runtime.Core.Effects
{
    public interface IEffect
    {
        bool TryRemoveEffect();
        void UpdateEffect(float deltaTime);
        bool IsExpired();
    }
}
