    using System.Collections.Generic;
    using UnityEngine;

    namespace TowerDefence.Runtime.Core.Pooling
    {
        public class ObjectPool<T> : IObjectPool<T> where T : Component
        {
            private readonly Dictionary<T, Queue<T>> _pools;
            private readonly Dictionary<T, HashSet<T>> _activeInstances;
            private readonly Dictionary<T, T> _instanceToPrefab;

            public ObjectPool()
            {
                _pools = new Dictionary<T, Queue<T>>();
                _activeInstances = new Dictionary<T, HashSet<T>>();
                _instanceToPrefab = new Dictionary<T, T>();
            }

            public T Get(T prefab)
            {
                if (prefab == null)
                    return null;

                T instance = GetFromPool(prefab);

                if (instance == null)
                {
                    instance = CreateInstance(prefab);
                    if (instance == null)
                    {
                        Debug.LogError($"Failed to create instance of {prefab.name}");
                        return null;
                    }
                }
                else
                {
                    instance.gameObject.SetActive(true);
                }

                // Track active instance
                if (!_activeInstances.ContainsKey(prefab))
                    _activeInstances[prefab] = new HashSet<T>();

                _activeInstances[prefab].Add(instance);
                _instanceToPrefab[instance] = prefab;

                // Call activation method if instance supports it
                if (instance is IPoolable poolable)
                    poolable.OnGet();

                return instance;
            }

            public void Return(T instance)
            {
                if (instance == null)
                    return;

                if (!_instanceToPrefab.TryGetValue(instance, out T prefab))
                {
                    Debug.LogWarning($"Trying to return instance {instance.name} that doesn't belong to this pool");
                    return;
                }

                // Remove from active tracking
                if (_activeInstances.ContainsKey(prefab))
                    _activeInstances[prefab].Remove(instance);

                // Call deactivation method if instance supports it
                if (instance is IPoolable poolable)
                    poolable.OnReturn();

                // Deactivate and return to pool
                instance.gameObject.SetActive(false);
                ReturnToPool(prefab, instance);
            }

            public void Prewarm(T prefab, int count)
            {
                if (prefab == null)
                {
                    Debug.LogError("Cannot prewarm pool with null prefab");
                    return;
                }

                if (!_pools.ContainsKey(prefab))
                    _pools[prefab] = new Queue<T>();

                for (int i = 0; i < count; i++)
                {
                    T instance = CreateInstance(prefab);
                    if (instance != null)
                    {
                        instance.gameObject.SetActive(false);
                        _pools[prefab].Enqueue(instance);
                    }
                }

                Debug.Log($"Prewarmed pool for {prefab.name} with {count} instances");
            }

            public void Clear()
            {
                foreach (var pool in _pools.Values)
                {
                    while (pool.Count > 0)
                    {
                        var instance = pool.Dequeue();
                        if (instance != null)
                            Object.Destroy(instance.gameObject);
                    }
                }

                _pools.Clear();
                _activeInstances.Clear();
                _instanceToPrefab.Clear();
            }

            public int GetPoolSize(T prefab)
            {
                return _pools.TryGetValue(prefab, out var pool) ? pool.Count : 0;
            }

            public int GetActiveCount(T prefab)
            {
                return _activeInstances.TryGetValue(prefab, out var instance) ? instance.Count : 0;
            }

            protected virtual T CreateInstance(T prefab)
            {
                return Object.Instantiate(prefab);
            }

            private T GetFromPool(T prefab)
            {
                if (!_pools.TryGetValue(prefab, out var pool))
                {
                    _pools[prefab] = new Queue<T>();
                    return null;
                }

                while (pool.Count > 0)
                {
                    var instance = pool.Dequeue();
                    if (instance != null)
                        return instance;
                }

                return null;
            }

            private void ReturnToPool(T prefab, T instance)
            {
                if (!_pools.ContainsKey(prefab))
                    _pools[prefab] = new Queue<T>();

                _pools[prefab].Enqueue(instance);
            }
        }
    }