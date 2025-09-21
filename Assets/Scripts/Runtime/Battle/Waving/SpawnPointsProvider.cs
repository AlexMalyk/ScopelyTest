using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Waving
{
    public class SpawnPointsProvider
    {
        private readonly SpawnPoint[] _spawnPoints;

        public int Count => _spawnPoints.Length;
        public SpawnPoint[] SpawnPoints => _spawnPoints;

        [Inject]
        public SpawnPointsProvider(SpawnPoint[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
        }

        public SpawnPoint GetSpawnPoint(int index)
        {
            return _spawnPoints[index];
        }

        public SpawnPoint GetRandomSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }
    }
}