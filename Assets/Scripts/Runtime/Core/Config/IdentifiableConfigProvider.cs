using System;
using VContainer;

namespace TowerDefence.Runtime.Config
{
    public class IdentifiableConfigProvider<T> where T : IdentifiableConfig
    {
        private readonly T[] _configs;

        [Inject]
        public IdentifiableConfigProvider(T[] configs)
        {
            _configs = configs;
        }

        public int Count => _configs.Length;

        public T GetByGuid(Guid guid)
        {
            foreach (var t in _configs)
                if (t.Id == guid)
                    return t;

            return null;
        }

        public T GetRandom()
        {
            return _configs[UnityEngine.Random.Range(0, _configs.Length)];
        }
    }
}