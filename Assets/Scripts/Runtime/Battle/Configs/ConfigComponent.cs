using System;
using TowerDefence.Runtime.Config;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [Serializable]
    public class ConfigComponent : EntityComponent
    {
        [SerializeField] private IdentifiableConfig _identifiableConfig;
        
        public Guid Id => _identifiableConfig.Id;
    }
}