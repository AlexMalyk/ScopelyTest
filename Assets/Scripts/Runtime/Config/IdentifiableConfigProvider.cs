using System;

namespace TowerDefence.Runtime.Config
{
    public class IdentifiableConfigProvider<T> where T : IdentifiableConfig
    {
        private readonly T[] _configs;

        public IdentifiableConfigProvider(T[] configs)
        {
            _configs = configs;
        }

        public int Count => _configs?.Length ?? 0;

        public T GetByGuid(Guid guid)
        {
            foreach (var t in _configs)
                if (t.Id == guid)
                    return t;

            return null;
        }
    }
}