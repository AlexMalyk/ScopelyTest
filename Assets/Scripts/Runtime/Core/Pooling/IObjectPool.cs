using UnityEngine;

namespace TowerDefence.Runtime.Core.Pooling
{
    public interface IObjectPool<T> where T : Component
    {
        T Get(T prefab);
        void Return(T instance);
        void Prewarm(T prefab, int count);
        void Clear();
        int GetPoolSize(T prefab);
        int GetActiveCount(T prefab);
    }
}