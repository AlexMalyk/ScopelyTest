using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityComponent[] _initialComponents;
        
        private Dictionary<Type, EntityComponent> _components = new();
        
        public event Action<EntityComponent> OnEntityComponentAdded;
        public event Action<EntityComponent> OnEntityComponentRemoving;

        [Inject]
        private void Initialize()
        {
            InitializeInitialComponents();
        }
    
        private void InitializeInitialComponents()
        {
            foreach (var component in _initialComponents) 
                AddEntityComponent(component);
        }

        public void AddEntityComponent(EntityComponent component)
        {
            var componentType = component.GetType();
            if (_components.TryAdd(componentType, component))
            {
                component.Initialize(this);
                
                OnEntityComponentAdded?.Invoke(component);
            }
        }

        public T GetEntityComponent<T>() where T : EntityComponent
        {
            var componentType = typeof(T);
            return _components.TryGetValue(componentType, out var component) ? (T)component : null;
        }

        public void RemoveEntityComponent<T>() where T : EntityComponent
        {
            var componentType = typeof(T);
            if (_components.TryGetValue(componentType, out var component))
            {
                OnEntityComponentRemoving?.Invoke(component);
                
                _components[componentType].OnDestroy();
                _components.Remove(componentType);
            }
        }
    
        private void OnDestroy()
        {
            foreach (var component in _components.Values) 
                component.OnDestroy();
            _components.Clear();
        }
    }
}