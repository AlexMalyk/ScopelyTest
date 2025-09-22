using TowerDefence.Runtime.Core.Entities;

namespace TowerDefence.Core.Effects
{
    public interface IEffectDefinition
    {
        EntityComponent CreateEffect();
    }
}