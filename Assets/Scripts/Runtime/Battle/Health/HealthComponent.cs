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
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
    
        private List<IHealthEffect> healthModifiers = new();

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            currentHealth = maxHealth;
            
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
            float modifiedDamage = damage;

            // Apply all health modifiers
            foreach (var modifier in healthModifiers)
            {
                modifiedDamage = modifier.ModifyDamage(modifiedDamage);
            }

            currentHealth = Mathf.Max(0, currentHealth - modifiedDamage);
        
            Debug.Log($"Took {modifiedDamage} damage (original: {damage}). Health: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                OnDeath();
            }
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            Debug.Log($"Healed {amount}. Health: {currentHealth}/{maxHealth}");
        }

        private void OnDeath()
        {
            Debug.Log("Entity died!");
        }
    }
}