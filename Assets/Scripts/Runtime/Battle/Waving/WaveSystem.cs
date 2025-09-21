using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TowerDefence.Runtime.Battle.Enemy;
using TowerDefence.Runtime.Core.Entities;
using TowerDefence.Runtime.Core.Pooling;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Waving
{
    public class WaveSystem : IStartable, IDisposable
    {
        private readonly WaveConfig[] _waveConfigs;
        private readonly SpawnPointsProvider _spawnPointsProvider;
        private readonly EntitySpawner _spawner;
        private readonly EntityPoolSystem _entityPoolSystem;

        // Events
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action OnAllWavesCompleted;
        public event Action<Component> OnEnemySpawned;
        
        // Private variables
        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private int activeEnemies = 0;
        private CancellationTokenSource _cancellationTokenSource;

        // Properties
        public int CurrentWave => currentWaveIndex + 1;
        public int TotalWaves => _waveConfigs.Length;
        public bool IsWaveActive => isSpawning || activeEnemies > 0;
        public int ActiveEnemies => activeEnemies;

        [Inject]
        public WaveSystem(EntitySpawner spawner, EntityPoolSystem entityPoolSystem, 
            SpawnPointsProvider spawnPointsProvider, WaveConfig[] waveConfigs)
        {
            _spawner = spawner;
            _entityPoolSystem = entityPoolSystem;
            _spawnPointsProvider = spawnPointsProvider;
            _waveConfigs = waveConfigs;
            
            Initialize();
        }

        void IStartable.Start()
        {
            StartWave();
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            InitializeObjectPools();
        }

        private void InitializeObjectPools()
        {
            Debug.Log("[WaveSystem] Starting pool prewarming...");

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
            if (currentWaveIndex >= _waveConfigs.Length || isSpawning)
                return;

            StartWaveAsync(_cancellationTokenSource.Token).Forget();
        }

        public void StartWave(int waveIndex)
        {
            if (waveIndex < 0 || waveIndex >= _waveConfigs.Length || isSpawning)
                return;

            currentWaveIndex = waveIndex;
            StartWave();
        }

        public void StopCurrentWave()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            isSpawning = false;
        }

        private async UniTaskVoid StartWaveAsync(CancellationToken cancellationToken)
        {
            if (currentWaveIndex >= _waveConfigs.Length)
                return;

            try
            {
                await SpawnWaveAsync(_waveConfigs[currentWaveIndex], cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Wave spawning was cancelled");
            }
        }

        private async UniTask SpawnWaveAsync(WaveConfig config, CancellationToken cancellationToken)
        {
            isSpawning = true;
            OnWaveStarted?.Invoke(CurrentWave);

            Debug.Log($"Starting Wave {CurrentWave}");
            
                // Spawn from multiple points simultaneously
            await SpawnSimultaneouslyAsync(config, cancellationToken);


            isSpawning = false;

            // Wait for all enemies to be defeated
            await UniTask.WaitUntil(() => activeEnemies <= 0, cancellationToken: cancellationToken);

            OnWaveCompleted?.Invoke(CurrentWave);
            Debug.Log($"Wave {CurrentWave} completed!");

            currentWaveIndex++;

            if (currentWaveIndex >= _waveConfigs.Length)
            {
                OnAllWavesCompleted?.Invoke();
                Debug.Log("All waves completed!");
            }
        }

        private async UniTask SpawnSimultaneouslyAsync(WaveConfig config, CancellationToken cancellationToken)
        {
            // Group spawn sequences by their timing
            var spawnTasks = new List<UniTask>();

            foreach (WaveSpawnSequence sequence in config.SpawnSequence)
            {
                spawnTasks.Add(SpawnSequenceAsync(sequence, config, cancellationToken));
            }

            // Wait for all spawn sequences to complete
            await UniTask.WhenAll(spawnTasks);
        }

        private async UniTask SpawnSequenceAsync(WaveSpawnSequence sequence, WaveConfig config,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < sequence.EnemyCount; i++)
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
            {
                return _spawnPointsProvider.GetSpawnPoint(spawnPointIndex);
            }

            // Use random spawn point
            if (spawnPointIndex == -1)
            {
                int randomIndex = UnityEngine.Random.Range(0, _spawnPointsProvider.Count);
                return _spawnPointsProvider.GetSpawnPoint(randomIndex);
            }

            // Default to first spawn point
            return _spawnPointsProvider.GetSpawnPoint(0);
        }

        private void SpawnEnemy(EnemyConfig enemyConfig, SpawnPoint spawnPoint)
        {
            var enemy = _spawner.Spawn(enemyConfig, spawnPoint.CachedTransform.position, spawnPoint.CachedTransform.rotation);
            if (enemy == null)
            {
                Debug.LogWarning($"Failed to spawn enemy: {enemyConfig.name}");
                return;
            }

            // Try to get enemy component and subscribe to death event
            //if (enemy.TryGetComponent<IEnemy>(out IEnemy enemyInterface))
            //{
            //    enemyInterface.OnEnemyDeath += HandleEnemyDeath;
            //}

            activeEnemies++;
            OnEnemySpawned?.Invoke(enemy);

            Debug.Log($"Spawned {enemy.gameObject.name} at {spawnPoint.name}. Active enemies: {activeEnemies}");
        }

        private void HandleEnemyDeath(Entity enemy)
        {
            activeEnemies = Mathf.Max(0, activeEnemies - 1);

            // Try to unsubscribe from death event
            //if (enemy.TryGetComponent<IEnemy>(out IEnemy enemyInterface))
            //{
            //    enemyInterface.OnEnemyDeath -= HandleEnemyDeath;
            //}

            _spawner.Despawn(enemy);

            Debug.Log($"Enemy defeated. Active enemies: {activeEnemies}");
        }

        void IDisposable.Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}