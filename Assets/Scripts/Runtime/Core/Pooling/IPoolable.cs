namespace TowerDefence.Runtime.Core.Pooling
{
    public interface IPoolable
    {
        void OnGet();
        void OnReturn();
    }
}