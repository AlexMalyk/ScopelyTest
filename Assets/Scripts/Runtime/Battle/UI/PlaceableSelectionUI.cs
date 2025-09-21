using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Economy;
using TowerDefence.Runtime.Battle.Placement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TowerDefence.Runtime.Core.Entities;
using VContainer;

namespace TowerDefence.Runtime.Battle.UI
{
    public class PlaceableSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private Button _cancelButton;
        
        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private float _statusDisplayTime = 3f;
        
        // Dependencies
        [Inject] private PlacementSystem _placementSystem;
        [Inject] private GoldSystem _goldSystem;
        [Inject] private PlaceableConfig[] _placeableConfigs;
        
        // State
        private List<PlaceableItemUI> _uiItems = new();
        private PlaceableItemUI _selectedItem;
        private Coroutine _statusCoroutine;
        
        private void Start()
        {
            Initialize();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void Initialize()
        {
            CreateUIItems();
            UpdateGoldDisplay();
            UpdateCancelButton();
            
            _statusText.gameObject.SetActive(false);
        }
        
        private void CreateUIItems()
        {
            if (_itemContainer == null || _itemPrefab == null)
            {
                Debug.LogWarning("PlaceableSelectionUI: Missing item container or prefab reference");
                return;
            }
            
            // Clear existing items
            ClearUIItems();
            
            // Create UI item for each placeable config
            foreach (var config in _placeableConfigs)
            {
                if (config == null) continue;
                
                var itemGO = Instantiate(_itemPrefab, _itemContainer);
                var uiItem = itemGO.GetComponent<PlaceableItemUI>();
                
                if (uiItem != null)
                {
                    uiItem.Initialize(config);
                    uiItem.OnItemSelected += OnItemSelected;
                    _uiItems.Add(uiItem);
                }
                else
                {
                    Debug.LogWarning($"Item prefab does not have PlaceableUIItem component: {config.DisplayName}");
                    Destroy(itemGO);
                }
            }
            
            // Update affordability for all items
            UpdateAllItemsAffordability();
        }
        
        private void ClearUIItems()
        {
            foreach (var item in _uiItems)
            {
                if (item != null)
                {
                    item.OnItemSelected -= OnItemSelected;
                    Destroy(item.gameObject);
                }
            }
            _uiItems.Clear();
        }
        
        private void SubscribeToEvents()
        {
            _goldSystem.OnGoldAmountChanged += OnGoldChanged;
            
            _placementSystem.OnPlacementStarted += OnPlacementStarted;
            _placementSystem.OnPlacementCancelled += OnPlacementCancelled;
            _placementSystem.OnEntityPlaced += OnEntityPlaced;
            _placementSystem.OnPlacementFailed += OnPlacementFailed;
            
            
            _cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void UnsubscribeFromEvents()
        {
            if(_goldSystem != null)
                _goldSystem.OnGoldAmountChanged -= OnGoldChanged;

            if (_placementSystem != null)
            {
                _placementSystem.OnPlacementStarted -= OnPlacementStarted;
                _placementSystem.OnPlacementCancelled -= OnPlacementCancelled;
                _placementSystem.OnEntityPlaced -= OnEntityPlaced;
                _placementSystem.OnPlacementFailed -= OnPlacementFailed;
            }

            _cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }
        
        private void OnItemSelected(PlaceableConfig config)
        {
            _placementSystem.StartPlacement(config);
        }
        
        private void OnGoldChanged(double newAmount)
        {
            UpdateGoldDisplay();
            UpdateAllItemsAffordability();
        }
        
        private void UpdateAllItemsAffordability()
        {
            foreach (var item in _uiItems)
            {
                if (item?.Config != null)
                {
                    var canAfford = _goldSystem.CanAfford(item.Config.Cost);
                    item.SetAffordable(canAfford);
                }
            }
        }
        
        private void OnPlacementStarted(PlaceableConfig config)
        {
            SetSelectedItem(config);
            UpdateCancelButton();
            ShowStatus($"Placing {config.DisplayName}... (Right-click or ESC to cancel)");
        }
        
        private void OnPlacementCancelled()
        {
            ClearSelectedItem();
            UpdateCancelButton();
        }
        
        private void OnEntityPlaced(Entity entity, PlaceableConfig config)
        {
            ShowStatus($"{config.DisplayName} placed! Cost: {config.Cost} gold");
        }
        
        private void OnPlacementFailed(string reason)
        {
            ShowStatus($"Placement failed: {reason}");
        }
        
        private void OnCancelButtonClicked()
        {
            _placementSystem.CancelPlacement();
        }
        
        private void SetSelectedItem(PlaceableConfig config)
        {
            ClearSelectedItem();
            
            // Find and select the new item
            foreach (var item in _uiItems)
            {
                if (item.Config == config)
                {
                    _selectedItem = item;
                    item.SetSelected(true);
                    break;
                }
            }
        }
        
        private void ClearSelectedItem()
        {
            if (_selectedItem != null)
            {
                _selectedItem.SetSelected(false);
                _selectedItem = null;
            }
        }
        
        private void UpdateGoldDisplay()
        {
            _goldText.text = $"Gold: {_goldSystem.GoldAmount:F0}";
        }
        
        private void UpdateCancelButton()
        {
            _cancelButton.gameObject.SetActive(_placementSystem.IsPlacing);
        }
        
        private void ShowStatus(string message)
        {
            if (_statusText == null) return;
            
            _statusText.text = message;
            _statusText.gameObject.SetActive(true);
            
            // Clear previous coroutine
            if (_statusCoroutine != null)
            {
                StopCoroutine(_statusCoroutine);
            }
            
            // Start new timer to hide status
            _statusCoroutine = StartCoroutine(HideStatusAfterDelay());
        }
        
        private System.Collections.IEnumerator HideStatusAfterDelay()
        {
            yield return new WaitForSeconds(_statusDisplayTime);
            
            if (_statusText != null)
            {
                _statusText.gameObject.SetActive(false);
            }
            
            _statusCoroutine = null;
        }
    }
}