using UnityEngine;
using TowerDefence.Runtime.Core.Entities;

namespace TowerDefence.Runtime.Battle.Targeting
{
    public interface ITargetable
    {
        /// <summary>
        /// The entity that implements this targetable interface
        /// </summary>
        Entity Entity { get; }
        
        /// <summary>
        /// The transform position for targeting calculations
        /// </summary>
        Transform TargetTransform { get; }
        
        /// <summary>
        /// Whether this target is currently valid (alive, active, etc.)
        /// </summary>
        bool IsValidTarget { get; }
        
        /// <summary>
        /// Priority for target selection (higher = more priority)
        /// Useful for different targeting strategies
        /// </summary>
        float TargetPriority { get; }
        
        /// <summary>
        /// Current health percentage (0-1) for targeting strategies that consider health
        /// </summary>
        float HealthPercentage { get; }
        
        /// <summary>
        /// Whether this target is an enemy (true for enemies, false for player units/structures)
        /// </summary>
        bool IsEnemy { get; }
    }
}