
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    [System.Serializable]
    public class FurthestTargetStrategy : ITargetingStrategy
    {
        public ITargetable SelectBestTarget(List<ITargetable> targets, Vector3 fromPosition)
        {
            switch (targets.Count)
            {
                case 0: return null;
                case 1: return targets[0];
            }

            var furthest = targets[0];
            var furthestDistance = Vector3.Distance(fromPosition, furthest.TargetTransform.position);

            for (var i = 1; i < targets.Count; i++)
            {
                float distance = Vector3.Distance(fromPosition, targets[i].TargetTransform.position);
                if (distance > furthestDistance)
                {
                    furthest = targets[i];
                    furthestDistance = distance;
                }
            }

            return furthest;
        }
    }
}