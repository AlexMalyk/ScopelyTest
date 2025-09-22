using System;
using TowerDefence.Runtime.Core.Entities;

namespace TowerDefence.Runtime.Battle.Buildings
{
    [Serializable]
    public class PlayerBaseComponent : EntityComponent
    {
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
        }
    }
}