using System;
using TowerDefence.Runtime.Battle.Configs;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefence.Runtime.Battle.UI
{
    public class PlaceableItemUI : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _costText;
        
        private PlaceableConfig _config;
        private bool _isAffordable;
        private bool _isSelected;
        
        public PlaceableConfig Config => _config;
        public bool IsAffordable => _isAffordable;
        public bool IsSelected => _isSelected;
        
        public event Action<PlaceableConfig> OnItemSelected;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        public void Initialize(PlaceableConfig config)
        {
            _config = config;
            UpdateUI();
        }
        
        public void SetAffordable(bool affordable)
        {
            if (_isAffordable != affordable)
            {
                _isAffordable = affordable;
                UpdateVisualState();
            }
        }
        
        public void SetSelected(bool selected)
        {
            if (_isSelected != selected)
            {
                _isSelected = selected;
                UpdateSelectionVisual();
            }
        }
        
        private void UpdateUI()
        {
            _nameText.text = _config.DisplayName;
            _costText.text = _config.Cost.ToString();
        }
        
        private void UpdateVisualState()
        {
            _button.interactable = _isAffordable;
        }
        
        private void UpdateSelectionVisual()
        {
            var colors = _button.colors;
            colors.selectedColor = _isSelected ? Color.yellow : colors.normalColor;
            _button.colors = colors;
        }
        
        private void OnButtonClicked()
        {
            if (_isAffordable) 
                OnItemSelected?.Invoke(_config);
        }
    }
}