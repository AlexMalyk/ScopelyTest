using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Projectiles
{
    public class ProjectileSystem : ITickable
    {
        private readonly HashSet<ProjectileComponent> _projectiles;
        private readonly List<ProjectileComponent> _pendingRemove;
        private readonly List<ProjectileComponent> _pendingAdd;
        private bool _isUpdating;
        
        [Inject]
        public ProjectileSystem()
        {
            _projectiles = new HashSet<ProjectileComponent>();
            _pendingRemove = new List<ProjectileComponent>();
            _pendingAdd = new List<ProjectileComponent>();
            _isUpdating = false;
        }
        
        public void RegisterProjectile(ProjectileComponent projectile)
        {
            if (projectile == null) return;
            if (_isUpdating)
            {
                // Defer additions until after the update loop to avoid modifying the set mid-iteration
                _pendingAdd.Add(projectile);
            }
            else
            {
                _projectiles.Add(projectile);
            }
        }
        
        public void UnregisterProjectile(ProjectileComponent projectile)
        {
            if (projectile == null) return;
            if (_isUpdating)
            {
                // Dirty-flag: mark for removal at the end of the update cycle
                _pendingRemove.Add(projectile);
            }
            else
            {
                _projectiles.Remove(projectile);
            }
        }
        
        void ITickable.Tick()
        {
            // Begin update phase: prevent structural modifications to _projectiles
            _isUpdating = true;

            foreach (var projectile in _projectiles)
            {
                if (projectile == null)
                    continue;

                if (projectile.IsActive)
                {
                    projectile.UpdateProjectile(Time.deltaTime);
                }

                // If projectile turned inactive during update, mark it for removal (dirty flag)
                if (!projectile.IsActive)
                {
                    _pendingRemove.Add(projectile);
                }
            }

            // End update phase; now it's safe to modify collections
            _isUpdating = false;

            // Apply batched removals
            if (_pendingRemove.Count > 0)
            {
                for (int i = 0; i < _pendingRemove.Count; i++)
                {
                    _projectiles.Remove(_pendingRemove[i]);
                }
                _pendingRemove.Clear();
            }

            // Apply batched additions
            if (_pendingAdd.Count > 0)
            {
                for (int i = 0; i < _pendingAdd.Count; i++)
                {
                    _projectiles.Add(_pendingAdd[i]);
                }
                _pendingAdd.Clear();
            }
        }
    }
}