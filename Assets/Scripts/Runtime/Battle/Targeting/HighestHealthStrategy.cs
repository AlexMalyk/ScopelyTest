using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [System.Serializable]
    public class HighestHealthTargetStrategy : ITargetingStrategy
    {
        public ITargetable SelectBestTarget(List<ITargetable> targets, Vector3 fromPosition)
        {
            switch (targets.Count)
            {
                case 0: return null;
                case 1: return targets[0];
            }

            var strongest = targets[0];
            var highestHealth = strongest.HealthPercentage;

            for (var i = 1; i < targets.Count; i++)
            {
                var health = targets[i].HealthPercentage;
                if (health > highestHealth)
                {
                    strongest = targets[i];
                    highestHealth = health;
                }
            }

            return strongest;
        }
    }
}