using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private Dictionary<Type, EntityComponent> _coreComponents = new();
        private Dictionary<Type, List<EntityComponent>> _effects = new();
        
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
                if (!_effects.TryGetValue(componentType, out var list))
                {
                    list = new List<EntityComponent>();
                    _effects[componentType] = list;
                }
                list.Add(component);
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
        
        public T[] GetEffects<T>() where T : EntityComponent
        {
            var targetType = typeof(T);

            // If we have an exact bucket, return it quickly
            if (_effects.TryGetValue(targetType, out var exactList))
                return exactList.Cast<T>().ToArray();

            // Otherwise, collect from all buckets whose key type derives from or implements T
            var result = new List<T>();
            foreach (var kvp in _effects)
            {
                if (targetType.IsAssignableFrom(kvp.Key))
                    result.AddRange(kvp.Value.Cast<T>());
            }

            return result.ToArray();
        }
        
        public T GetEffect<T>(Predicate<T> predicate) where T : EntityComponent
        {
            var effects = GetEffects<T>();
            return Array.Find(effects, predicate);
        }
        
        public bool RemoveEffect(EntityComponent component)
        {
            if (component == null) return false;
            var type = component.GetType();
            if (!_effects.TryGetValue(type, out var list) || list == null) return false;

            int index = list.IndexOf(component);
            if (index < 0) return false;

            OnEntityComponentRemoving?.Invoke(component);

            component.Cleanup();

            list.RemoveAt(index);
            if (list.Count == 0)
                _effects.Remove(type);

            return true;
        }
        
        public void ResetEntity()
        {
            foreach (var coreComponent in _coreComponents.Values) 
                coreComponent.Reset();
            
            foreach (var effectsList in _effects.Values)
            {
                foreach (var effectComponent in effectsList)
                    effectComponent.Reset();
            }
        }
        
        public void CleanupEntity()
        {
            foreach (var coreComponent in _coreComponents.Values) 
                coreComponent.Cleanup();

            foreach (var effectsList in _effects.Values)
            {
                foreach (var effectComponent in effectsList)
                    effectComponent.Cleanup();
            }
            
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