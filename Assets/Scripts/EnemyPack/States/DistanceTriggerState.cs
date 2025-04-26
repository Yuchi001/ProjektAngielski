using System;
using Utils;

namespace EnemyPack.States
{
    public class DistanceTriggerState : StateBase
    {
        private readonly float _distance;
        private readonly Action<EnemyLogic> _onPlayerEnterAction;
        private readonly Action<EnemyLogic> _onPlayerExitAction;

        private bool _wasInRange = false;

        public DistanceTriggerState(float distance, Action<EnemyLogic> onPlayerEnterAction, Action<EnemyLogic> onPlayerExitAction = null)
        {
            _distance = distance;
            _onPlayerEnterAction = onPlayerEnterAction;
            _onPlayerExitAction = onPlayerExitAction;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _wasInRange = false;
        }

        public override void Execute(EnemyLogic state)
        {
            if (!state.transform.InRange(PlayerPos, _distance))
            {
                if (!_wasInRange || _onPlayerExitAction == null) return;

                _onPlayerExitAction.Invoke(state);
                return;
            }
            
            _onPlayerEnterAction.Invoke(state);
            _wasInRange = true;
        }

        public override void Reset(EnemyLogic state)
        {
            _wasInRange = false;
        }
    }
}