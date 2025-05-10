using EnemyPack.SO;

namespace EnemyPack.States
{
    public abstract class DefaultStateBase : StateBase
    {
        protected DefaultStateBase(SoEnemy data) : base(data)
        {
        }

        public sealed override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            // IGNORE
        }
    }
}