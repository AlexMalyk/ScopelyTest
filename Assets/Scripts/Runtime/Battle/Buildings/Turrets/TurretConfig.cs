using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Turrets
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Configs/Turret")]
    public class TurretConfig : IdentifiableConfig
    {
        [Header("Turret Config")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _cost;
        
        public GameObject Prefab => _prefab;
        public int Cost => _cost;
    }
}
