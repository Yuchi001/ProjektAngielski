using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class BatSmallMain : StateBase, IRootState
    {
        private const float IDLE_RANGE = 4f;
        private const float PATROL_RANGE = 4f;
        private const float DETECTION_RANGE = 4f;

        private Chase _chaseState;
        private Patrol _patrolState;
        
        public void Compose(EnemyLogic logic)
        {
            _chaseState = new Chase().SetOutOfRangeAction(enemyLogic => enemyLogic.SwitchState(_patrolState), IDLE_RANGE);
            _patrolState = new Patrol(PATROL_RANGE).SetOnPlayerInRange(DETECTION_RANGE, enemyLogic => enemyLogic.SwitchState(_chaseState));
        }

        public override void Enter(EnemyLogic state)
        {
            state.SwitchState(_patrolState);
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}