using System;
using TowerDefence.Runtime.Core.Entities;

namespace TowerDefence.Runtime.Battle.Placement
{
    [Serializable]
    public class PlaceableComponent : EntityComponent
    {
        private PlaceableState _placementState = PlaceableState.Preview;
        
        public event Action<PlaceableComponent> OnPlacementStateChanged;

        public override void Reset()
        {
            base.Reset();
            SetPlacementState(PlaceableState.Preview);
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