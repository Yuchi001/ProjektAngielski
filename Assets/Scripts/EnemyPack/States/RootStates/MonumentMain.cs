using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class MonumentMain : StateBase, IRootState
    {
        private const float SHOOT_RANGE = 4;
        
        public void Compose(EnemyLogic logic)
        {
            
        }
        
        public override void Enter(EnemyLogic state)
        {
            var animator = state.Animator;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, 0f);
            animator.speed = 0f;
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            if (!InRange(state, SHOOT_RANGE)) return;

            state.Animator.speed = 1f;
            //TODO: implement shooting
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}