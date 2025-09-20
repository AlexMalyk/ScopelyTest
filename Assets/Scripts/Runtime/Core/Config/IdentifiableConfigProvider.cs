using System;
using System.Collections.Generic;
using VContainer;

namespace TowerDefence.Runtime.Config
{
    public class IdentifiableConfigProvider<T> where T : IdentifiableConfig
    {
        private readonly T[] _configs;
        private readonly Dictionary<Guid, T> _configLookup;
        
        public int Count => _configs.Length;

        [Inject]
        public IdentifiableConfigProvider(T[] configs)
        {
            _configs = configs;
            _configLookup = new Dictionary<Guid, T>();
        
            foreach (var config in configs)
            {
                if (config != null)
                    _configLookup[config.Id] = config;
            }
        }

        public T GetByGuid(Guid guid)
        {
            return _configLookup.GetValueOrDefault(guid);
        }
        
        public T GetRandom()
        {
            return _configs[UnityEngine.Random.Range(0, _configs.Length)];
        }
    }
}