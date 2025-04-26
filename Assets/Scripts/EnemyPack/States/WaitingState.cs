using System;

namespace EnemyPack.States
{
    public class WaitingState : StateBase
    {
        private readonly float _time;
        private readonly Action<EnemyLogic> _onWaitOverAction;

        private float _timer = 0;

        public WaitingState(float time, Action<EnemyLogic> onWaitOverAction)
        {
            _time = time;
            _onWaitOverAction = onWaitOverAction;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _timer = 0;
        }

        public override void Execute(EnemyLogic state)
        {
            _timer += state.deltaTime;
            if (_timer < _time) return;
            
            _onWaitOverAction.Invoke(state);
        }

        public override void Reset(EnemyLogic state)
        {
            _timer = 0;
        }
    }
}