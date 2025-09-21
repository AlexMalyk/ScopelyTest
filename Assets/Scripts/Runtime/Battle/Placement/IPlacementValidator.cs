using TowerDefence.Runtime.Battle.Configs;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Placement
{
    public interface IPlacementValidator
    {
        bool IsValidPlacement(Vector3 position, PlaceableConfig config);
        Vector3 GetAdjustedPosition(Vector3 inputPosition, PlaceableConfig config);
    }
}