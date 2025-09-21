using System;
using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Economy;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Placement
{
    public class PlacementSystem : ITickable, IDisposable
    {
        private enum SystemPlacementState
        {
            Idle,
            Placing
        }
        
        private readonly EntitySpawner _entitySpawner;
        private readonly IPlacementValidator _placementValidator;
        private readonly GoldSystem _goldSystem;
        private readonly Camera _camera;
        
        // Placement state
        private SystemPlacementState _currentState = SystemPlacementState.Idle;
        private PlaceableConfig _currentConfig;
        private Entity _previewEntity;
        private PlaceableComponent _previewComponent;
        
        // Tracking
        private readonly Dictionary<Guid, List<Entity>> _placedEntitiesByConfig = new();
        
        // Events
        public event Action<PlaceableConfig> OnPlacementStarted;
        public event Action OnPlacementCancelled;
        public event Action<Entity, PlaceableConfig> OnEntityPlaced;
        public event Action<string> OnPlacementFailed;
        
        // Properties
        public bool IsPlacing => _currentState == SystemPlacementState.Placing;
        public PlaceableConfig CurrentConfig => _currentConfig;
        
        [Inject]
        public PlacementSystem(EntitySpawner entitySpawner, IPlacementValidator placementValidator, GoldSystem goldSystem)
        {
            _entitySpawner = entitySpawner;
            _placementValidator = placementValidator;
            _goldSystem = goldSystem;
            _camera = Camera.main;
        }
        
        void ITickable.Tick()
        {
            if (_currentState == SystemPlacementState.Placing)
            {
                HandlePlacementInput();
                UpdatePreviewPosition();
            }
        }
        
        public void StartPlacement(PlaceableConfig config)
        {
            if (config == null)
            {
                Debug.LogWarning("Cannot start placement with null config");
                return;
            }
            
            // Check if player can afford this placeable
            if (!_goldSystem.CanAfford(config.Cost))
            {
                OnPlacementFailed?.Invoke($"Not enough gold! Need {config.Cost}, have {_goldSystem.GoldAmount}");
                return;
            }
            
            // Cancel any existing placement
            if (_currentState == SystemPlacementState.Placing)
            {
                CancelPlacement();
            }
            
            _currentConfig = config;
            _currentState = SystemPlacementState.Placing;
            
            CreatePreviewEntity();
            OnPlacementStarted?.Invoke(config);
            
            Debug.Log($"Started placement of {config.DisplayName}");
        }
        
        public void CancelPlacement()
        {
            if (_currentState != SystemPlacementState.Placing)
                return;
                
            DestroyPreviewEntity();
            
            _currentConfig = null;
            _currentState = SystemPlacementState.Idle;
            
            OnPlacementCancelled?.Invoke();
            Debug.Log("Placement cancelled");
        }
        
        public int GetPlacedCount(PlaceableConfig config)
        {
            if (config == null) return 0;
            
            return _placedEntitiesByConfig.TryGetValue(config.Id, out var entities) ? entities.Count : 0;
        }
        
        public List<Entity> GetPlacedEntities(PlaceableConfig config)
        {
            if (config == null) return new List<Entity>();
            
            return _placedEntitiesByConfig.TryGetValue(config.Id, out var entities) 
                ? new List<Entity>(entities) 
                : new List<Entity>();
        }
        
        private void HandlePlacementInput()
        {
            // Cancel on ESC or right mouse button
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
                return;
            }
            
            // Place on left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceAtMousePosition();
            }
        }
        
        private void UpdatePreviewPosition()
        {
            if (_previewEntity == null || _camera == null)
                return;
                
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);
            
            // Raycast to get world position
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var adjustedPosition = _placementValidator.GetAdjustedPosition(hit.point, _currentConfig);
                _previewEntity.CachedTransform.position = adjustedPosition;
                
                // Update preview visual state based on validity
                UpdatePreviewVisuals(adjustedPosition);
            }
        }
        
        private void UpdatePreviewVisuals(Vector3 position)
        {
            if (_previewComponent == null) return;
            
            var isValid = _placementValidator.IsValidPlacement(position, _currentConfig);
            
            // This could be expanded later to change materials/colors based on validity
            // For now, we just ensure the preview is visible
            _previewEntity.gameObject.SetActive(true);
        }
        
        private void TryPlaceAtMousePosition()
        {
            if (_camera == null)
            {
                OnPlacementFailed?.Invoke("No camera found for placement");
                return;
            }
            
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var adjustedPosition = _placementValidator.GetAdjustedPosition(hit.point, _currentConfig);
                
                if (_placementValidator.IsValidPlacement(adjustedPosition, _currentConfig))
                {
                    PlaceEntity(adjustedPosition);
                }
                else
                {
                    OnPlacementFailed?.Invoke("Cannot place here - invalid location");
                }
            }
            else
            {
                OnPlacementFailed?.Invoke("Cannot place here - no valid surface");
            }
        }
        
        private void PlaceEntity(Vector3 position)
        {
            // Deduct cost
            _goldSystem.SpendGold(_currentConfig.Cost);
            
            // Spawn the actual entity
            var placedEntity = _entitySpawner.Spawn(_currentConfig, position, Quaternion.identity);
            
            if (placedEntity != null)
            {
                // Ensure the placed entity has a PlaceableComponent and mark it as placed
                var placeableComponent = placedEntity.GetEntityComponent<PlaceableComponent>();
                if (placeableComponent == null)
                {
                    // Add component if not present
                    placeableComponent = new PlaceableComponent();
                    placeableComponent.SetConfig(_currentConfig);
                    placedEntity.AddEntityComponent(placeableComponent);
                }
                
                placeableComponent.Place();
                
                // Track the placed entity
                TrackPlacedEntity(placedEntity, _currentConfig);
                
                OnEntityPlaced?.Invoke(placedEntity, _currentConfig);
                Debug.Log($"Placed {_currentConfig.DisplayName} at {position}");
            }
            else
            {
                // Refund if spawn failed
                _goldSystem.AddGold(_currentConfig.Cost);
                OnPlacementFailed?.Invoke("Failed to spawn entity");
            }
            
            // Continue placement mode (don't cancel automatically)
            // Players can place multiple of the same type
        }
        
        private void CreatePreviewEntity()
        {
            if (_currentConfig == null) return;
            
            _previewEntity = _entitySpawner.Spawn(_currentConfig, Vector3.zero, Quaternion.identity);
            
            if (_previewEntity != null)
            {
                _previewComponent = _previewEntity.GetEntityComponent<PlaceableComponent>();
                if (_previewComponent == null)
                {
                    _previewComponent = new PlaceableComponent();
                    _previewComponent.SetConfig(_currentConfig);
                    _previewEntity.AddEntityComponent(_previewComponent);
                }
                
                _previewComponent.SetPreview();
                
                // Disable colliders on preview to prevent interference
                var colliders = _previewEntity.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
        
        private void DestroyPreviewEntity()
        {
            if (_previewEntity != null)
            {
                _entitySpawner.Despawn(_previewEntity);
                _previewEntity = null;
                _previewComponent = null;
            }
        }
        
        private void TrackPlacedEntity(Entity entity, PlaceableConfig config)
        {
            var configId = config.Id;
            
            if (!_placedEntitiesByConfig.ContainsKey(configId))
            {
                _placedEntitiesByConfig[configId] = new List<Entity>();
            }
            
            _placedEntitiesByConfig[configId].Add(entity);
        }
        
        void IDisposable.Dispose()
        {
            CancelPlacement();
            _placedEntitiesByConfig.Clear();
        }
    }
}