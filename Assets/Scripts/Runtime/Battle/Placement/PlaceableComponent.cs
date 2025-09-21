using System;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Placement
{
    [Serializable]
    public class PlaceableComponent : EntityComponent
    {
        [SerializeField] private PlaceableConfig _config;

        private PlaceableState _placementState = PlaceableState.Preview;

        public PlaceableConfig Config => _config;
        public PlaceableState PlacementState => _placementState;
        public bool IsPlaced => _placementState == PlaceableState.Placed;
        public bool IsPreview => _placementState == PlaceableState.Preview;

        public event Action<PlaceableComponent> OnPlacementStateChanged;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);

            if (_config == null)
            {
                Debug.LogWarning($"PlaceableComponent on {entity.name} has no PlaceableConfig assigned");
            }
        }

        public override void Reset()
        {
            base.Reset();
            SetPlacementState(PlaceableState.Preview);
        }

        public void SetConfig(PlaceableConfig config)
        {
            _config = config;
        }

        public void SetPlacementState(PlaceableState state)
        {
            if (_placementState != state)
            {
                _placementState = state;
                OnPlacementStateChanged?.Invoke(this);
            }
        }

        public void Place()
        {
            SetPlacementState(PlaceableState.Placed);
        }

        public void SetPreview()
        {
            SetPlacementState(PlaceableState.Preview);
        }
    }
}