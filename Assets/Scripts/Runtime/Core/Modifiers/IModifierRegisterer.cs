namespace TowerDefence.Runtime.Core.Modifiers
{
    public interface IModifierRegisterer<T> where T : IModifier
    {
        void RegisterModifier(T modifier);
        void UnregisterModifier(T modifier);
    }
}