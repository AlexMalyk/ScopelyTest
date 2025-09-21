using System;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Economy
{
    public class GoldSystem
    {
        private double _goldAmount = 5;

        public double GoldAmount => _goldAmount;

        public Action<double> OnGoldAmountChanged;
        
        public void SpendGold(double amount)
        {
            _goldAmount -= amount;
            OnGoldAmountChanged?.Invoke(_goldAmount);
        }

        public void AddGold(double amount)
        {
            _goldAmount += amount;
            OnGoldAmountChanged?.Invoke(_goldAmount);
        }

        public bool CanAfford(double amount) =>
            _goldAmount >= amount;
    }
}