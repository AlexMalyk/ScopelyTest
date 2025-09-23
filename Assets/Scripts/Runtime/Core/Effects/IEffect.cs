namespace TowerDefence.Runtime.Core.Effects
{
    public interface IEffect
    {
        void UpdateEffect(float deltaTime);
        bool IsExpired();
    }
}
