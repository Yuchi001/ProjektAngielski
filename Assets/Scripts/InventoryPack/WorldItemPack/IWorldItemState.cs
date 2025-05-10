namespace InventoryPack.WorldItemPack
{
    public interface IWorldItemState
    {
        public void Enter(WorldItem item);
        public void Execute(WorldItem item);
        public void LazyExecute(WorldItem item, float deltaTime);
    }
}