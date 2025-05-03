using EnemyPack.SO;
using EnemyPack.States.StateData;
using PlayerPack;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyPack.States
{
    public class MeleeAttackState : StateBase
    {
        private readonly MeleeAttackStateData meleeAttackStateData;
        private StateBase _lastState;

        private float AttackSpeed => 1f / meleeAttackStateData.AttackRate;

        private float _timer = -1;
        
        public MeleeAttackState(SoEnemy data) : base(data)
        {
            meleeAttackStateData = data.GetStateData<MeleeAttackStateData>();
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _lastState = lastState;
            if (_timer < 0) _timer = AttackSpeed;
        }

        public override void Execute(EnemyLogic state)
        {
            if (!InRange(state, meleeAttackStateData.AttackRange))
            {
                state.SwitchState(_lastState);
                return;
            }
            
            _lastState.Execute(state);
            _timer += state.deltaTime;
            if (_timer < AttackSpeed) return;

            _timer = 0;
            PlayerManager.PlayerHealth.GetDamaged(meleeAttackStateData.Damage);
        }

        public override void Reset(EnemyLogic state)
        {
            _timer = -1;
        }
    }
}