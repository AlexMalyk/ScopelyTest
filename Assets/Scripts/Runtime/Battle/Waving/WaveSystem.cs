using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Battle.Enemies;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Core.Pooling;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Waving
{
    public class WaveSystem : IDisposable
    {
        private readonly WaveConfig[] _waveConfigs;
        private readonly SpawnPointsProvider _spawnPointsProvider;
        private readonly EntitySpawner _spawner;
        private readonly EntityPoolSystem _entityPoolSystem;
        private readonly EnemyTrackerSystem _enemyTrackerSystem;

        public event Action OnAllWavesCompleted;
        
        private int _currentWaveIndex;
        private bool _isSpawning;
        private CancellationTokenSource _cancellationTokenSource;

        public int CurrentWave => _currentWaveIndex + 1;

        [Inject]
        public WaveSystem(EntitySpawner spawner, EntityPoolSystem entityPoolSystem,
            SpawnPointsProvider spawnPointsProvider, WaveConfig[] waveConfigs, EnemyTrackerSystem enemyTrackerSystem)
        {
            _spawner = spawner;
            _entityPoolSystem = entityPoolSystem;
            _spawnPointsProvider = spawnPointsProvider;
            _waveConfigs = waveConfigs;
            _enemyTrackerSystem = enemyTrackerSystem;
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            InitializeObjectPools();
        }

        private void InitializeObjectPools()
        {
            var enemyConfigCounts = new Dictionary<EnemyConfig, int>();

            foreach (var waveConfig in _waveConfigs)
            {
                foreach (var sequence in waveConfig.SpawnSequence)
                {
                    if (sequence.EnemyConfig != null)
                    {
                        enemyConfigCounts.TryAdd(sequence.EnemyConfig, 0);

                        // Use the max count from any single wave as the prewarm amount
                        enemyConfigCounts[sequence.EnemyConfig] =
                            Mathf.Max(enemyConfigCounts[sequence.EnemyConfig], sequence.EnemyCount);
                    }
                }
            }
            
            // Prewarm pools for each enemy config
            foreach (var kvp in enemyConfigCounts) 
                _entityPoolSystem.Prewarm(kvp.Key, kvp.Value);
            
            Debug.Log($"[WaveSystem] Pool prewarming complete. Prewarmed {enemyConfigCounts.Count} different enemy types.");
        }

        public void StartWave()
        {
            if(_isSpawning)
                return;

            if (_currentWaveIndex >= _waveConfigs.Length)
            {
                OnAllWavesCompleted?.Invoke();
                Debug.Log("All waves completed!");
                return;
            }

            StartWaveAsync(_cancellationTokenSource.Token).Forget();
        }

        public void StopCurrentWave()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _isSpawning = false;
        }

        private async UniTaskVoid StartWaveAsync(CancellationToken cancellationToken)
        {
            if (_currentWaveIndex >= _waveConfigs.Length)
                return;

            try
            {
                await SpawnWaveAsync(_waveConfigs[_currentWaveIndex], cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Wave spawning was cancelled");
            }
            
            _currentWaveIndex++;
        }

        private async UniTask SpawnWaveAsync(WaveConfig config, CancellationToken cancellationToken)
        {
            _isSpawning = true;

            Debug.Log($"Starting Wave {CurrentWave}");
            
            await SpawnSimultaneouslyAsync(config, cancellationToken);
            
            _isSpawning = false;
        }

        private async UniTask SpawnSimultaneouslyAsync(WaveConfig config, CancellationToken cancellationToken)
        {
            // Group spawn sequences by their timing
            var spawnTasks = new List<UniTask>();

            foreach (var sequence in config.SpawnSequence) 
                spawnTasks.Add(SpawnSequenceAsync(sequence, config, cancellationToken));

            // Wait for all spawn sequences to complete
            await UniTask.WhenAll(spawnTasks);
        }

        private async UniTask SpawnSequenceAsync(WaveSpawnSequence sequence, WaveConfig config,
            CancellationToken cancellationToken)
        {
            for (var i = 0; i < sequence.EnemyCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var spawnPoint = GetSpawnPoint(sequence.SpawnPointIndex);
                SpawnEnemy(sequence.EnemyConfig, spawnPoint);

                if (config.SpawnInterval > 0 && i < sequence.EnemyCount - 1)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(config.SpawnInterval),
                        cancellationToken: cancellationToken);
                }
            }
        }

        private SpawnPoint GetSpawnPoint(int spawnPointIndex)
        {
            // If specific spawn point is requested and valid
            if (spawnPointIndex >= 0 && spawnPointIndex < _spawnPointsProvider.Count)
                return _spawnPointsProvider.GetSpawnPoint(spawnPointIndex);

            // Use random spawn point
            if (spawnPointIndex == -1)
            {
                int randomIndex = UnityEngine.Random.Range(0, _spawnPointsProvider.Count);
                return _spawnPointsProvider.GetSpawnPoint(randomIndex);
            }

            return _spawnPointsProvider.GetSpawnPoint(0);
        }

        private void SpawnEnemy(EnemyConfig enemyConfig, SpawnPoint spawnPoint)
        {
            var enemy = _spawner.Spawn(enemyConfig, 
                spawnPoint.CachedTransform.position, 
                spawnPoint.CachedTransform.rotation);
            _enemyTrackerSystem.TrackEnemy(enemy);
        }
        
        void IDisposable.Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}