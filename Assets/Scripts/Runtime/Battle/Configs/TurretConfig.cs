using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Configs/Turret")]
    public class TurretConfig : IdentifiableConfig
    {
        [Header("Turret Config")]
        [SerializeField] private int _cost;
        
        public int Cost => _cost;
    }
}
