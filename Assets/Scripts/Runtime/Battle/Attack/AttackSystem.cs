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
            
            var targeting = attacker.Entity.GetCoreEntityComponent<TargetingComponent>();
            if (targeting != null) 
                _targeters.Add(targeting);
        }

        public void UnregisterAttacker(AttackComponent attacker)
        {
            _attackers.Remove(attacker);
            
            var targeting = attacker.Entity.GetCoreEntityComponent<TargetingComponent>();
            if (targeting != null) 
                _targeters.Remove(targeting);
        }

        void ITickable.Tick()
        {
            foreach (var targeter in _targeters) 
                targeter.UpdateTargeting();

            foreach (var attacker in _attackers) 
                attacker.UpdateAttack();
        }
    }
}