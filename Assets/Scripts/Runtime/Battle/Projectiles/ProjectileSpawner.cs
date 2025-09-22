using System;
using TowerDefence.Runtime.Battle.Configs;
using TowerDefence.Runtime.Core.Entities;
using UnityEngine;
using VContainer;

namespace TowerDefence.Runtime.Battle.Projectiles
{
    public class ProjectileSpawner
    {
        private readonly EntitySpawner _entitySpawner;
        
        [Inject]
        public ProjectileSpawner(EntitySpawner entitySpawner)
        {
            _entitySpawner = entitySpawner;
        }
        
        public ProjectileComponent SpawnProjectile(ProjectileConfig config, Vector3 spawnPosition, 
            Entity target, float damage, Action<Entity> onHitCallback = null)
        {
            var projectileEntity = _entitySpawner.Spawn(config, spawnPosition, Quaternion.identity);
            
            if (projectileEntity == null)
            {
                Debug.LogWarning($"Failed to spawn projectile: {config.DisplayName}");
                return null;
            }
            
            var projectileBehavior = projectileEntity.GetCoreEntityComponent<ProjectileComponent>();
            if (projectileBehavior == null)
            {
                Debug.LogError($"Spawned projectile entity does not have ProjectileBehaviorComponent: {config.DisplayName}");
                _entitySpawner.Despawn(projectileEntity);
                return null;
            }
            
            projectileBehavior.Launch(target, damage, onHitCallback, OnProjectileDestroyed);
            
            return projectileBehavior;
        }
        
        public ProjectileComponent SpawnProjectile(ProjectileConfig config, Vector3 spawnPosition, 
            Vector3 targetPosition, float damage, Action<Entity> onHitCallback = null)
        {
            var projectileEntity = _entitySpawner.Spawn(config, spawnPosition, Quaternion.identity);
            
            if (projectileEntity == null)
            {
                Debug.LogWarning($"Failed to spawn projectile: {config.DisplayName}");
                return null;
            }
            
            var projectileBehavior = projectileEntity.GetCoreEntityComponent<ProjectileComponent>();
            if (projectileBehavior == null)
            {
                Debug.LogError($"Spawned projectile entity does not have ProjectileBehaviorComponent: {config.DisplayName}");
                _entitySpawner.Despawn(projectileEntity);
                return null;
            }
            
            projectileBehavior.Launch(targetPosition, damage, onHitCallback, OnProjectileDestroyed);
            
            return projectileBehavior;
        }
        
        private void OnProjectileDestroyed(Entity projectileEntity)
        {
            if (projectileEntity != null) 
                _entitySpawner.Despawn(projectileEntity);
        }
    }
}