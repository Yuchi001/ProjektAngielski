using UnityEngine;

namespace EnemyPack.States.BatSmall
{
    public class BatSmallMain : StateBase
    {
        public override bool CanBeStunned => true;
        public override bool CanBePushed => true;

        private static readonly float IDLE_RANGE = 4f;

        public override void Enter(EnemyLogic state)
        {
            
        }

        public override void Execute(EnemyLogic state)
        {
            if (Vector2.Distance(state.transform.position, PlayerPos) > IDLE_RANGE) return;
            
            state.SwitchState(new Chase());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}