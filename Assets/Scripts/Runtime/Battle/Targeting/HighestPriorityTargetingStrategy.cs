using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [System.Serializable]
    public class HighestPriorityTargetStrategy : ITargetingStrategy
    {
        public ITargetable SelectBestTarget(List<ITargetable> targets, Vector3 fromPosition)
        {
            switch (targets.Count)
            {
                case 0: return null;
                case 1: return targets[0];
            }

            var highestPriority = targets[0];
            var maxPriority = highestPriority.TargetPriority;

            for (var i = 1; i < targets.Count; i++)
            {
                var priority = targets[i].TargetPriority;
                if (priority > maxPriority)
                {
                    highestPriority = targets[i];
                    maxPriority = priority;
                }
            }

            return highestPriority;
        }
    }
}