using System;
using TowerDefence.Runtime.Battle.Enemies;
using TowerDefence.Runtime.Battle.Waving;
using UnityEngine;
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

        [Inject]
        public BattleLoopSystem(GameObject winPopup, GameObject losePopup, WaveSystem waveSystem)
        {
            _winPopup = winPopup;
            _losePopup = losePopup;
            
            _waveSystem = waveSystem;
        }

        void IStartable.Start()
        {
            _waveSystem.OnAllWavesCompleted += ShowWinPopup;
            _enemyTrackerSystem.OnAllEnemiesEliminated += AdvanceLoop;
            
            AdvanceLoop();
        }

        private void AdvanceLoop()
        {
            _waveSystem.StartWave();
        }

        private void ShowWinPopup()
        {
            _winPopup.SetActive(true);
        }

        private void ShowLosePopup()
        {
            _losePopup.SetActive(true);
        }

        void IDisposable.Dispose()
        {
            if (_waveSystem != null) 
                _waveSystem.OnAllWavesCompleted -= ShowWinPopup;
            
            if(_enemyTrackerSystem != null)
                _enemyTrackerSystem.OnAllEnemiesEliminated -= AdvanceLoop;
        }
    }
}