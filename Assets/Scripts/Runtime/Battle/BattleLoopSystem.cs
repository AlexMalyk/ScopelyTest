using System;
using Cysharp.Threading.Tasks;
using TowerDefence.Runtime.Battle.Buildings.PlayerBase;
using TowerDefence.Runtime.Battle.Enemies;
using TowerDefence.Runtime.Battle.Waving;
using TowerDefence.Runtime.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle
{
    public class BattleLoopSystem : IStartable, IDisposable
    {
        private readonly GameObject _winPopup;
        private readonly GameObject _losePopup;
        private readonly WaveSystem _waveSystem;
        private readonly EnemyTrackerSystem _enemyTrackerSystem;
        private readonly PlayerBaseHealthSystem _playerBaseHealthSystem;
        private readonly SceneLoader _sceneLoader;

        [Inject]
        public BattleLoopSystem(GameObject winPopup, GameObject losePopup, 
            WaveSystem waveSystem, EnemyTrackerSystem enemyTrackerSystem, 
            PlayerBaseHealthSystem playerBaseHealthSystem, SceneLoader sceneLoader)
        {
            _winPopup = winPopup;
            _losePopup = losePopup;
            
            _waveSystem = waveSystem;
            _enemyTrackerSystem = enemyTrackerSystem;
            _playerBaseHealthSystem = playerBaseHealthSystem;
            _sceneLoader = sceneLoader;
        }

        void IStartable.Start()
        {
            _waveSystem.OnAllWavesCompleted += ShowWinPopup;
            _enemyTrackerSystem.OnAllEnemiesEliminated += AdvanceLoop;
            _playerBaseHealthSystem.OnPlayerBaseDestroyed += ShowLosePopup;
            
            AdvanceLoop();
        }

        private void AdvanceLoop()
        {
            _waveSystem.StartWave();
        }

        private void ShowWinPopup()
        {
            _winPopup.SetActive(true);
            LoadMainMenu().Forget();
        }

        private void ShowLosePopup()
        {
            _losePopup.SetActive(true);
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            await UniTask.Delay(100);
            
            _sceneLoader.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        void IDisposable.Dispose()
        {
            if (_waveSystem != null) 
                _waveSystem.OnAllWavesCompleted -= ShowWinPopup;
            
            if(_enemyTrackerSystem != null)
                _enemyTrackerSystem.OnAllEnemiesEliminated -= AdvanceLoop;

            if (_playerBaseHealthSystem != null) 
                _playerBaseHealthSystem.OnPlayerBaseDestroyed -= ShowLosePopup;
        }
    }
}