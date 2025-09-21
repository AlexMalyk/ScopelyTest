using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence.Runtime.Battle.Health
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        
        public void SetHealth(float health, float maxHealth)
        {
            _slider.maxValue = maxHealth;
            _slider.value = health;
        }
    }
}