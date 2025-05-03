using EnemyPack.SO;

namespace EnemyPack.States
{
    /// <summary>
    /// State that can be used in composition with other states at the same time
    /// </summary>
    public abstract class CompositionStateBase : StateBase
    {
        protected CompositionStateBase(SoEnemy data) : base(data)
        {
        }

        public sealed override void Enter(EnemyLogic state, StateBase lastState)
        {
            // IGNORE
        }

        public sealed override void Reset(EnemyLogic state)
        {
            // IGNORE
        }
    }
}