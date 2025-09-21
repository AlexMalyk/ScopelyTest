using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Core.Entities
{
    public class Entity : MonoBehaviour
    {
        [Inject] private IObjectResolver _objectResolver;

        [SerializeField] private Transform _view;
        [SerializeField] private Transform _transform;
        
        [SerializeReference, SubclassSelector] private List<EntityComponent> _initialComponents = new();
        private Dictionary<Type, EntityComponent> _components = new();

        public Transform CachedTransform => transform;
        public Transform View => _view;
        
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
                _objectResolver.Inject(component);
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
                
                component.Cleanup();
                _components.Remove(componentType);
            }
        }
        
        public void ResetEntity()
        {
            foreach (var component in _initialComponents) 
                component.Reset();
        }
        
        public void CleanupEntity()
        {
            foreach (var component in _initialComponents) 
                component.Cleanup();
        }

        public void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public void OnDespawn()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            CleanupEntity();
            _components.Clear();
        }
    }
}