using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Health
{
    public interface IHealthEffect
    {
        void PlayHitEffect();
        void PlayDeathEffect();
        void ShowHealthBar(bool show);
    }
}
