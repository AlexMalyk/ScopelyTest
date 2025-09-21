using System;
using TowerDefence.Runtime.Battle.Enemy;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Waving
{
    [Serializable]
    public class WaveSpawnSequence
    {
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private int _enemyCount;
        [SerializeField, Tooltip("-1 means random spawn point")] private int _spawnPointIndex = -1;
        
        public EnemyConfig EnemyConfig => _enemyConfig;
        public int EnemyCount => _enemyCount;
        public int SpawnPointIndex => _spawnPointIndex;
    }
}