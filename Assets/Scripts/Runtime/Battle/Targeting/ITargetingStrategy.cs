using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Targeting
{
    public interface ITargetingStrategy
    {
        ITargetable SelectBestTarget(List<ITargetable> targets, Vector3 fromPosition);
    }
}