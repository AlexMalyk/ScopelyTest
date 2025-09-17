using System;

namespace TowerDefence.Runtime.Battle.Health
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(int damage);
        event Action OnDeath;
    }
}