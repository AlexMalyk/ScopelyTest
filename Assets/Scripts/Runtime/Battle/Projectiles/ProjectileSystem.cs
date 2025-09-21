using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TowerDefence.Runtime.Battle.Projectiles
{
    public class ProjectileSystem : ITickable
    {
        private HashSet<ProjectileComponent> _projectiles;
        private List<ProjectileComponent> _projectilesToRemove;
        
        [Inject]
        public ProjectileSystem()
        {
            _projectiles = new HashSet<ProjectileComponent>();
            _projectilesToRemove = new List<ProjectileComponent>();
        }
        
        public void RegisterProjectile(ProjectileComponent projectile)
        {
            _projectiles.Add(projectile);
        }
        
        public void UnregisterProjectile(ProjectileComponent projectile)
        {
            _projectiles.Remove(projectile);
        }
        
        void ITickable.Tick()
        {
            // Update all active projectiles
            foreach (var projectile in _projectiles)
            {
                if (projectile.IsActive)
                {
                    projectile.UpdateProjectile(Time.deltaTime);
                    
                    // Mark inactive projectiles for cleanup tracking
                    if (!projectile.IsActive)
                    {
                        _projectilesToRemove.Add(projectile);
                    }
                }
            }
            
            // Clear the removal list (actual despawning is handled by ProjectileSpawner)
            if (_projectilesToRemove.Count > 0)
            {
                _projectilesToRemove.Clear();
            }
        }
    }
}