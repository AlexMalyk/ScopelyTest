using TowerDefence.Runtime.Config;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Configs
{
    [CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/Projectile")]
    public class ProjectileConfig : IdentifiableConfig
    {
        [Header("Projectile Settings")]
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseSpeed = 10f;
        [SerializeField] private float _lifeTime = 5f;
        
        public float BaseDamage => _baseDamage;
        public float BaseSpeed => _baseSpeed;
        public float LifeTime => _lifeTime;
    }
}