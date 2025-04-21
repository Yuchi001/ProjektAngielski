using System.Numerics;

namespace EnemyPack.States.RootStates
{
    public class BatSmallShootMain : StateBase, IRootState
    {
        private const float PATROL_RANGE = 4f;
        private const float DETECTION_RANGE = 3f;

        private Patrol _patrolState;
        
        public void Compose(EnemyLogic logic)
        {
            _patrolState = new Patrol(PATROL_RANGE, enemyLogic => { }).SetOnPlayerInRange(DETECTION_RANGE, enemyLogic => enemyLogic.SwitchState(_patrolState)); // TODO: implement shoot
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