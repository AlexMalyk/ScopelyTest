using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Waving;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Configs/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Header("Spawn Settings")] 
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private List<WaveSpawnSequence> _spawnSequence = new();

        public float SpawnInterval => _spawnInterval;
        public List<WaveSpawnSequence> SpawnSequence => _spawnSequence;
    }
}