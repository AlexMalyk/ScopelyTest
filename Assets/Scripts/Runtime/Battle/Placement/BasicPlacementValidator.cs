using TowerDefence.Runtime.Battle.Configs;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Placement
{
    public class BasicPlacementValidator : IPlacementValidator
    {
        private readonly LayerMask _groundLayer;
        private readonly float _maxGroundDistance;
        
        public BasicPlacementValidator(LayerMask groundLayer = default, float maxGroundDistance = 100f)
        {
            _groundLayer = groundLayer == 0 ? 1 : groundLayer; // Default to layer 0 if not specified
            _maxGroundDistance = maxGroundDistance;
        }
        
        public bool IsValidPlacement(Vector3 position, PlaceableConfig config)
        {
            if (config == null)
            {
                Debug.LogWarning("PlaceableConfig is null in placement validation");
                return false;
            }
            
            // Check if position is on valid ground
            if (!IsOnValidGround(position))
            {
                return false;
            }
            
            // Check for overlapping objects using the config's bounds and blocking layers
            var adjustedPosition = GetAdjustedPosition(position, config);
            return !HasBlockingObjects(adjustedPosition, config);
        }
        
        public Vector3 GetAdjustedPosition(Vector3 inputPosition, PlaceableConfig config)
        {
            // Raycast down to find ground position
            var rayStart = inputPosition + Vector3.up * _maxGroundDistance;
            
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, _maxGroundDistance * 2f, _groundLayer))
            {
                // Place on ground surface, accounting for placeable bounds
                var groundPosition = hit.point;
                var boundsOffset = config.PlacementBoundsSize.y * 0.5f;
                return groundPosition + Vector3.up * boundsOffset;
            }
            
            // If no ground found, return original position
            Debug.LogWarning($"No ground found at position {inputPosition}");
            return inputPosition;
        }
        
        private bool IsOnValidGround(Vector3 position)
        {
            // Raycast down from above the position to check for ground
            var rayStart = position + Vector3.up * _maxGroundDistance;
            return Physics.Raycast(rayStart, Vector3.down, _maxGroundDistance * 2f, _groundLayer);
        }
        
        private bool HasBlockingObjects(Vector3 position, PlaceableConfig config)
        {
            // Use OverlapBox to check for collisions with blocking objects
            var bounds = config.PlacementBoundsSize;
            var blockingLayers = config.BlockingLayers;
            
            // Check if any colliders overlap with the placement bounds
            var overlapping = Physics.OverlapBox(position, bounds * 0.5f, Quaternion.identity, blockingLayers);
            
            return overlapping.Length > 0;
        }
    }
}