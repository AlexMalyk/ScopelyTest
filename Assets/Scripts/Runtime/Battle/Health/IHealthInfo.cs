using System;

namespace TowerDefence.Runtime.Battle.Health
{
    public interface IHealthInfo : IDamageable
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        float HealthPercentage { get; }
        event Action<int, int> OnDamageTaken; // damage, currentHealth
    }
}