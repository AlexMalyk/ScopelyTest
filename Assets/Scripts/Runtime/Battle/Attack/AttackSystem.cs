using System.Collections.Generic;
using TowerDefence.Runtime.Battle.Targeting;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Attack
{
    public class AttackSystem : ITickable
    {
        private HashSet<AttackComponent> _attackers;
        private HashSet<TargetingComponent> _targeters;

        [Inject]
        public AttackSystem()
        {
            _attackers = new HashSet<AttackComponent>();
            _targeters = new HashSet<TargetingComponent>();
        }

        public void RegisterAttacker(AttackComponent attacker)
        {
            _attackers.Add(attacker);
            
            // Also register its targeting component
            var targeting = attacker.Entity.GetEntityComponent<TargetingComponent>();
            if (targeting != null)
            {
                _targeters.Add(targeting);
            }
        }

        public void UnregisterAttacker(AttackComponent attacker)
        {
            _attackers.Remove(attacker);
            
            // Also unregister its targeting component
            var targeting = attacker.Entity.GetEntityComponent<TargetingComponent>();
            if (targeting != null)
            {
                _targeters.Remove(targeting);
            }
        }

        void ITickable.Tick()
        {
            // Update targeting for all attackers
            foreach (var targeter in _targeters)
            {
                targeter.UpdateTargeting();
            }

            // Update attacks for all attackers
            foreach (var attacker in _attackers)
            {
                attacker.UpdateAttack();
            }
        }
    }
}