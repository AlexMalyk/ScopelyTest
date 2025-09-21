using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Movement
{
    public class MovementSystem : ITickable
    {
        private HashSet<MovementComponent> _components;

        [Inject]
        public MovementSystem()
        {
            _components = new HashSet<MovementComponent>();
        }

        public void RegisterComponent(MovementComponent component)
        {
            _components.Add(component);
        }

        public void UnregisterComponent(MovementComponent component)
        {
            _components.Remove(component);
        }

        void ITickable.Tick()
        {
            foreach (var component in _components)
            {
                component.Move();
            }
        }
    }
}