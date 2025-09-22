using System;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Economy;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

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
        
        private SystemPlacementState _currentState = SystemPlacementState.Idle;
        private PlaceableConfig _currentConfig;
        private GameObject _previewGameObject;

        public event Action<PlaceableConfig> OnPlacementStarted;
        public event Action OnPlacementCancelled;
        public event Action<Entity, PlaceableConfig> OnEntityPlaced;
        public event Action<string> OnPlacementFailed;
        
        public bool IsPlacing => _currentState == SystemPlacementState.Placing;
        
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
            if (!_goldSystem.CanAfford(config.Cost))
            {
                OnPlacementFailed?.Invoke($"Not enough gold! Need {config.Cost}, have {_goldSystem.GoldAmount}");
                return;
            }
            
            if (_currentState == SystemPlacementState.Placing) 
                CancelPlacement();
            
            _currentConfig = config;
            _currentState = SystemPlacementState.Placing;
            
            CreatePreviewGameObject();
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
                TryPlaceAtMousePosition();
        }
        
        private void UpdatePreviewPosition()
        {
            if (_previewGameObject == null || _camera == null)
                return;
                
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);
            
            // Raycast to get world position
            if (Physics.Raycast(ray, out var hit))
            {
                var adjustedPosition = _placementValidator.GetAdjustedPosition(hit.point, _currentConfig);
                _previewGameObject.transform.position = adjustedPosition;
                _previewGameObject.gameObject.SetActive(true);
            }
        }
        
        private void TryPlaceAtMousePosition()
        {
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var adjustedPosition = _placementValidator.GetAdjustedPosition(hit.point, _currentConfig);

                if (_placementValidator.IsValidPlacement(adjustedPosition, _currentConfig))
                    PlaceEntity(adjustedPosition);
                else
                    OnPlacementFailed?.Invoke("Cannot place here - invalid location");
            }
            else
                OnPlacementFailed?.Invoke("Cannot place here - no valid surface");
        }
        
        private void PlaceEntity(Vector3 position)
        {
            
            var placedEntity = _entitySpawner.Spawn(_currentConfig, position, Quaternion.identity);
            if (placedEntity != null)
            {
                var placeableComponent = placedEntity.GetCoreEntityComponent<PlaceableComponent>();
                
                placeableComponent.Place();
                
                OnEntityPlaced?.Invoke(placedEntity, _currentConfig);
                Debug.Log($"Placed {_currentConfig.DisplayName} at {position}");
                _goldSystem.SpendGold(_currentConfig.Cost);
            }
            else
                OnPlacementFailed?.Invoke("Failed to spawn entity");

            CancelPlacement();
        }
        
        private void CreatePreviewGameObject()
        {
            if (_currentConfig == null) return;
            
            _previewGameObject = Object.Instantiate(_currentConfig.PreviewPrefab);
        }
        
        private void DestroyPreviewEntity()
        {
            if (_previewGameObject == null) return;
            
            Object.Destroy(_previewGameObject);
            _previewGameObject = null;
        }
        
        void IDisposable.Dispose()
        {
            CancelPlacement();
        }
    }
}