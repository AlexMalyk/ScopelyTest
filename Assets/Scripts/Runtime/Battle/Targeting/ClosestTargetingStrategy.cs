using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [System.Serializable]
    public class ClosestTargetStrategy : ITargetingStrategy
    {
        public ITargetable SelectBestTarget(List<ITargetable> targets, Vector3 fromPosition)
        {
            switch (targets.Count)
            {
                case 0: return null;
                case 1: return targets[0];
            }

            var closest = targets[0];
            var closestDistance = Vector3.Distance(fromPosition, closest.TargetTransform.position);

            for (var i = 1; i < targets.Count; i++)
            {
                var distance = Vector3.Distance(fromPosition, targets[i].TargetTransform.position);
                if (distance < closestDistance)
                {
                    closest = targets[i];
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}