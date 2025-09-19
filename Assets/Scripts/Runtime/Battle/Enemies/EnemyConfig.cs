using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Enemy
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/Enemy")]
    public class EnemyConfig : IdentifiableConfig
    {
        [Header("Enemy Config")]
        
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _reward = 0;
        
        public GameObject Prefab => _prefab;
        public int Reward => _reward;
    }
}
