using TowerDefence.Runtime.Battle.Health;
using TowerDefence.Runtime.Battle.Movement;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        //[SerializeField] private EnemyType _enemyType;

        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private MovementComponent _movementComponent;

        private bool _isInitialized;
        private Transform _cachedTransform;

        //public EnemyType Type => _enemyType;
        public bool IsAlive => _healthComponent.IsAlive;
        public Vector3 Position => _cachedTransform.position;
        public Transform Transform => _cachedTransform;

        public System.Action<EnemyController> OnEnemyDestroyed;
        public System.Action<EnemyController> OnEnemyHit;

        public MovementComponent MovementComponent => _movementComponent;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        [Inject]
        private void Init()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            InitializeComponents();

            _healthComponent.Death += OnDeath;
            _healthComponent.DamageTaken += OnDamageTaken;

            _isInitialized = true;
        }

        public void InitializeComponents()
        {
            _healthComponent.Initialize();
            //_movementComponent.Initialize();
        }

        public void TakeDamage(int damage = 1)
        {
            _healthComponent.TakeDamage(damage);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void OnDamageTaken(int damage, int currentHealth)
        {
            OnEnemyHit?.Invoke(this);
            _healthComponent.PlayHitEffect();
        }

        private void OnDeath()
        {
            OnEnemyDestroyed?.Invoke(this);

            _healthComponent.Death -= OnDeath;
            _healthComponent.DamageTaken -= OnDamageTaken;
        }
    }
}