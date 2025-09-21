using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [CreateAssetMenu(fileName = "PlaceableConfig", menuName = "Configs/Placeable")]
    public class PlaceableConfig : IdentifiableConfig
    {
        [Header("Placeable Settings")]
        [SerializeField] private int _cost = 5;
        [SerializeField] private Vector3 _placementBoundsSize = Vector3.one;
        [SerializeField] private LayerMask _blockingLayers = -1;
        
        public int Cost => _cost;
        public Vector3 PlacementBoundsSize => _placementBoundsSize;
        public LayerMask BlockingLayers => _blockingLayers;
    }
}