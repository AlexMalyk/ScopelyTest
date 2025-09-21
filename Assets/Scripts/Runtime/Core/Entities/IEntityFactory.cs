namespace TowerDefence.Runtime.Core.Entities
{
    public interface IEntityFactory
    {
        Entity CreateEntity(Entity prefab);
    }
}