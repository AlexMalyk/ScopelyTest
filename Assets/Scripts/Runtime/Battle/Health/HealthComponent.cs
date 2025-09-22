using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Core.Efefcts;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Health
{
    [Serializable]
    public class HealthComponent : EntityComponent, IEntityComponentListener, IEffectRegisterer<IHealthEffect>
    {
        [SerializeField] private HealthBar _healthBar;
        
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
    
        private List<IHealthEffect> healthModifiers = new();

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        
        public event Action<Entity, HealthComponent> OnDeath;
        public event Action<Entity, HealthComponent> OnEliminated;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            SetHealth(maxHealth);
            
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
            if (!healthModifiers.Contains(effect)) 
                healthModifiers.Add(effect);
        }

        public void UnregisterEffect(IHealthEffect effect)
        {
            healthModifiers.Remove(effect);
        }

        public void TakeDamage(float damage)
        {
            var modifiedDamage = damage;

            foreach (var modifier in healthModifiers) 
                modifiedDamage = modifier.ModifyDamage(modifiedDamage);

            SetHealth(Mathf.Max(0, currentHealth - modifiedDamage));
        
            Debug.Log($"Took {modifiedDamage} damage (original: {damage}). Health: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0) 
                OnDeath?.Invoke(_entity, this);
        }

        public void Eliminate()
        {
            OnEliminated?.Invoke(_entity, this);
        }

        public void Heal(float amount)
        {
            SetHealth(Mathf.Min(maxHealth, currentHealth + amount));
            Debug.Log($"Healed {amount}. Health: {currentHealth}/{maxHealth}");
        }

        private void SetHealth(float health)
        {
            currentHealth = health;
            _healthBar?.SetHealth(currentHealth, maxHealth);
        }

        public override void Reset()
        {
            base.Reset();
            
            healthModifiers = new List<IHealthEffect>();
            SetHealth(maxHealth);
        }
    }
}