using System;
using UnityEngine;

namespace TowerDefence.Runtime.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] protected int _maxHealth = 1;

        protected int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        
        public event Action<int, int> HealthChanged; // (current, max)
        public event Action<int, int> DamageTaken; // (damage, currentHealth)
        public event Action Death;
        
        protected virtual void Awake() { }

        public virtual void Initialize()
        {
            _currentHealth = _maxHealth;
            InitializeVisuals();
        }

        protected virtual void InitializeVisuals() { }

        public virtual void TakeDamage(int damage = 1)
        {
            if (!IsAlive) return;

            var oldHealth = _currentHealth;
            _currentHealth = Mathf.Max(0, _currentHealth - damage);

            DamageTaken?.Invoke(damage, _currentHealth);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);

            HandleDamage(damage, oldHealth, _currentHealth);

            if (_currentHealth <= 0) 
                Die();
        }
        
        protected virtual void HandleDamage(int damage, int oldHealth, int newHealth) { }

        public virtual void Heal(int amount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public virtual void SetMaxHealth(int newMaxHealth)
        {
            _maxHealth = newMaxHealth;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        protected virtual void Die()
        {
            Death?.Invoke();
        }
        
        public virtual void PlayHitEffect() { }
    }
}