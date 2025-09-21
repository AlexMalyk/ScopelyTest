using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/Enemy")]
    public class EnemyConfig : IdentifiableConfig
    {
        [Header("Enemy Config")]
        [SerializeField] private int _reward = 0;
        
        public int Reward => _reward;
    }
}
