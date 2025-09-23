using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Effects;
using TowerDefence.Runtime.Core.Efefcts;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Health
{
    [Serializable]
    public class HealthComponent : EntityComponent, IEntityComponentListener, IEffectRegisterer<IHealthEffect>
    {
        [SerializeField] private HealthBar _healthBar;
        
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
    
        private List<IHealthEffect> _healthModifiers = new();

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        
        public event Action<Entity, HealthComponent> OnDeath;
        public event Action<Entity, HealthComponent> OnEliminated;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            SetHealth(_maxHealth);
            
            entity.OnEntityComponentAdded += OnEntityComponentAdded;
            entity.OnEntityComponentRemoving += OnEntityComponentRemoving;
        }

        public void OnEntityComponentAdded(EntityComponent component)
        {
            if (component is IHealthEffect healthModifier) 
                RegisterEffect(healthModifier);
        }

        public void OnEntityComponentRemoving(EntityComponent component)
        {
            if (component is IHealthEffect healthModifier) 
                UnregisterEffect(healthModifier);
        }

        public void RegisterEffect(IHealthEffect effect)
        {
            if (!_healthModifiers.Contains(effect)) 
                _healthModifiers.Add(effect);
        }

        public void UnregisterEffect(IHealthEffect effect)
        {
            _healthModifiers.Remove(effect);
        }

        public void TakeDamage(float damage)
        {
            var modifiedDamage = damage;

            foreach (var modifier in _healthModifiers) 
                modifiedDamage = modifier.ModifyDamage(modifiedDamage);

            SetHealth(Mathf.Max(0, _currentHealth - modifiedDamage));
        
            Debug.Log($"Took {modifiedDamage} damage (original: {damage}). Health: {_currentHealth}/{_maxHealth}");

            if (_currentHealth <= 0) 
                OnDeath?.Invoke(_entity, this);
        }

        public void Eliminate()
        {
            OnEliminated?.Invoke(_entity, this);
        }

        public void Heal(float amount)
        {
            SetHealth(Mathf.Min(_maxHealth, _currentHealth + amount));
            Debug.Log($"Healed {amount}. Health: {_currentHealth}/{_maxHealth}");
        }

        private void SetHealth(float health)
        {
            _currentHealth = health;
            _healthBar?.SetHealth(_currentHealth, _maxHealth);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            
            _healthModifiers.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            
            SetHealth(_maxHealth);
        }
    }
}