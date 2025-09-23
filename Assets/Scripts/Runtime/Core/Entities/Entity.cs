using System;
using System.Collections.Generic;
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
        
        [Header("Editor Setup")]
        [SerializeReference, SubclassSelector] private List<EntityComponent> _initialComponents = new();
        [SerializeReference, SubclassSelector] private List<IEffectDefinition> _initialEffects = new();
        
        [Header("Runtime")]
        [SerializeReference, SubclassSelector] private List<EntityComponent> _coreComponents = new();
        [SerializeReference, SubclassSelector] private List<EntityComponent> _effects = new();
        
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
                if(!_coreComponents.Contains(component))
                    _coreComponents.Add(component);
                else
                {
                    Debug.LogWarning($"Core component {componentType.Name} already exists!");
                    return;
                }
            }
            else
            {
                if(!_effects.Contains(component))
                    _effects.Add(component);
                else
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
            foreach (var component in _coreComponents)
            {
                if (component is T entityComponent)
                    return entityComponent;
            }

            return null;
        }
        
        public T GetEffect<T>() where T : EntityComponent
        {
            foreach (var effect in _effects)
            {
                if (effect is T entityComponent)
                    return entityComponent;
            }

            return null;
        }
        
        public bool TryRemoveEffect(EntityComponent component)
        {
            if (component == null) return false;
            
            if (!_effects.Contains(component)) return false;

            OnEntityComponentRemoving?.Invoke(component);

            component.Cleanup();
            
            _effects.Remove(component);
            
            return true;
        }
        
        public void ResetEntity()
        {
            foreach (var coreComponent in _coreComponents) 
                coreComponent.Reset();
            
            foreach (var effectComponent in _effects)
                effectComponent.Reset();
        }
        
        public void CleanupEntity()
        {
            foreach (var coreComponent in _coreComponents) 
                coreComponent.Cleanup();

            foreach (var effectComponent in _effects)
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