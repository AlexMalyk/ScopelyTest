using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Enemy;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Waving
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Configs/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Wave Settings")] 
        [Header("Enemy Configuration")] 
        [SerializeField] private List<EnemyConfig> _enemyConfigs = new();

        [Header("Spawn Settings")] 
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private List<WaveSpawnSequence> _spawnSequence = new();

        public List<EnemyConfig> EnemyConfigs => _enemyConfigs;
        public float SpawnInterval => _spawnInterval;
        public List<WaveSpawnSequence> SpawnSequence => _spawnSequence;
    }
}