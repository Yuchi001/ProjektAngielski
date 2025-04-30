using System;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using PlayerPack;

namespace EnemyPack.States
{
    public class LookAtTriggerState : CompositionStateBase
    {
        private readonly Func<StateBase> _nextState;
        private readonly ELookType _lookType;
        
        public LookAtTriggerState(SoEnemy data, Func<StateBase> nextState, ELookType lookType) : base(data)
        {
            _lookType = lookType;
            _nextState = nextState;
        }

        public override void Execute(EnemyLogic state)
        {
            var pos = state.transform.position;
            var playerLookingRight = PlayerManager.PlayerMovement.LookingRight;
            var looksAtMe = (playerLookingRight && pos.x > PlayerPos.x) || (!playerLookingRight && pos.x < PlayerPos.x);
            if (looksAtMe && _lookType == ELookType.LOOK_AT) state.SwitchState(_nextState.Invoke());
            if (!looksAtMe && _lookType == ELookType.LOOK_AWAY) state.SwitchState(_nextState.Invoke());
        }

        public enum ELookType
        {
            LOOK_AT,
            LOOK_AWAY
        }
    }
}