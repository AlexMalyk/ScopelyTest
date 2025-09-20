using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Core.Modifiers;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Health
{
    [Serializable]
    public class HealthComponent : EntityComponent, IEntityComponentListener, IModifierRegisterer<IHealthModifier>
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
    
        private List<IHealthModifier> healthModifiers = new();

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
            if (component is IHealthModifier healthModifier) 
                RegisterModifier(healthModifier);
        }

        public void OnEntityComponentRemoving(EntityComponent component)
        {
            if (component is IHealthModifier healthModifier) 
                UnregisterModifier(healthModifier);
        }

        public void RegisterModifier(IHealthModifier modifier)
        {
            if (!healthModifiers.Contains(modifier)) 
                healthModifiers.Add(modifier);
        }

        public void UnregisterModifier(IHealthModifier modifier)
        {
            healthModifiers.Remove(modifier);
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