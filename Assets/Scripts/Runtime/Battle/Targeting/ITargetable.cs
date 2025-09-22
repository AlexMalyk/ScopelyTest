using UnityEngine;
using TowerDefence.Runtime.Core.Entities;

namespace TowerDefence.Runtime.Battle.Targeting
{
    public interface ITargetable
    {
        Entity Entity { get; }
        
        Transform TargetTransform { get; }
        
        bool IsValidTarget { get; }
        
        float TargetPriority { get; }
        
        float HealthPercentage { get; }
        
        bool IsEnemy { get; }
    }
}