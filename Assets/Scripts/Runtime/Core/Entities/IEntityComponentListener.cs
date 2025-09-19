namespace TowerDefence.Runtime.Core.Entities
{
    public interface IEntityComponentListener
    {
        void OnEntityComponentAdded(EntityComponent component);
        void OnEntityComponentRemoving(EntityComponent component);
    }
}