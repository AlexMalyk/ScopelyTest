using TowerDefence.Runtime.Core.Entities;
using UnityEngine;

namespace TowerDefence.Runtime.Battle.Movement
{
    public class MultiplyMovementModifier : EntityComponent, IMovementModifier
    {
        [SerializeField] private float _multiplier = 0.5f;
        [SerializeField] private float _duration = 3f;
        
        private float _timeRemaining;

        public float ModifyMovement(float movementSpeed)
        {
            return movementSpeed * _multiplier;
        }

        protected virtual void Start()
        {
            _timeRemaining = _duration;
        }

        protected virtual void Update()
        {
            _timeRemaining -= Time.deltaTime;
            
            if (_timeRemaining <= 0) 
                _entity.RemoveEntityComponent<MultiplyMovementModifier>();
        }
    }
}
