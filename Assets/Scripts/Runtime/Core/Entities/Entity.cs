using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefence.Core.Effects;
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
        [SerializeReference, SubclassSelector] private List<IEffectDefinition> _initialEffects = new();
        
        private Dictionary<Type, EntityComponent> _coreComponents = new();
        private Dictionary<Type, EntityComponent> _effects = new();
        
        public Transform CachedTransform => _transform;
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
                AddEntityComponent(component, true);
            
            foreach (var effect in _initialEffects)
                AddEntityComponent(effect.CreateEffect());
        }

        public void AddEntityComponent(EntityComponent component, bool isCoreComponent = false)
        {
            var componentType = component.GetType();
            
            if (isCoreComponent)
            {
                if (!_coreComponents.TryAdd(componentType, component))
                {
                    Debug.LogWarning($"Core component {componentType.Name} already exists!");
                    return;
                }
            }
            else
            {
                if (!_effects.TryAdd(componentType, component))
                {
                    Debug.LogWarning($"Effect component {componentType.Name} already exists!");
                    return;
                }
            }
            
            _objectResolver.Inject(component);
            component.Initialize(this);
                
            OnEntityComponentAdded?.Invoke(component);
        }

        public T GetCoreEntityComponent<T>() where T : EntityComponent
        {
            // Fast path: exact type
            if (_coreComponents.TryGetValue(typeof(T), out var exact))
                return (T)exact;

            // Fallback: find the first component whose concrete type is assignable to T
            var targetType = typeof(T);
            foreach (var kvp in _coreComponents)
            {
                if (targetType.IsAssignableFrom(kvp.Key))
                    return (T)kvp.Value;
            }

            return null;
        }
        
        public T GetEffect<T>() where T : EntityComponent
        {
            // Fast path: exact type
            if (_effects.TryGetValue(typeof(T), out var exact))
                return (T)exact;

            // Fallback: find the first component whose concrete type is assignable to T
            var targetType = typeof(T);
            foreach (var kvp in _effects)
            {
                if (targetType.IsAssignableFrom(kvp.Key))
                    return (T)kvp.Value;
            }

            return null;
        }
        
        public bool TryRemoveEffect(EntityComponent component)
        {
            if (component == null) return false;
            var type = component.GetType();
            
            if (!_effects.ContainsKey(type)) return false;

            OnEntityComponentRemoving?.Invoke(component);

            component.Cleanup();
            
            _effects.Remove(type);
            return true;
        }
        
        public void ResetEntity()
        {
            foreach (var coreComponent in _coreComponents.Values) 
                coreComponent.Reset();
            
            foreach (var effectComponent in _effects.Values)
                effectComponent.Reset();
            
            _coreComponents.Clear();
        }
        
        public void CleanupEntity()
        {
            foreach (var coreComponent in _coreComponents.Values) 
                coreComponent.Cleanup();

            foreach (var effectComponent in _effects.Values)
                effectComponent.Cleanup();
            
            _effects.Clear();
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
            _coreComponents.Clear();
            _effects.Clear();
        }
    }
}