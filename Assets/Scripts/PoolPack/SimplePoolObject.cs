namespace PoolPack
{
    public abstract class SimplePoolObject : PoolObject
    {
        protected sealed override void LazyUpdate(float lazyDeltaTime)
        {
            // IGNORE
        }
    }
}