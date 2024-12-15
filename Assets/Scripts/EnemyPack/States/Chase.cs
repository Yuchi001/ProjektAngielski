using System;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using PlayerPack;
using UnityEngine;

namespace EnemyPack.States
{
    public class Chase : StateBase
    {
        public override bool CanBeStunned => _canBeStunned;
        private bool _canBeStunned = true;
        public override bool CanBePushed => _canBePushed;
        private bool _canBePushed = true;

        private Action<EnemyLogic> _onPlayerInRange = null;

        private float _range = 0.5f;

        private SoEnemy _data;

        private Transform _transform;

        private const string CHASE_ATTACK_TIMER_ID = "CHASE_ATTACK_TIMER";
        private const string CHASE_MOVE_TIMER_ID = "CHASE_MOVE_TIMER";

        private new static Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;

        public Chase(StateBase nextState)
        {
            _onPlayerInRange = logic => logic.SwitchState(nextState);
        }

        public Chase()
        {
            _onPlayerInRange = null;
        }

        public Chase SetRange(float range)
        {
            _range = range;
            return this;
        }

        public Chase SetCanBePushed(bool canBePushed)
        {
            _canBePushed = canBePushed;
            return this;
        }
        
        public Chase SetCanBeStunned(bool canBeStunned)
        {
            _canBeStunned = canBeStunned;
            return this;
        }

        public Chase SetOnPlayerInRange(Action<EnemyLogic> onPlayerInRange)
        {
            _onPlayerInRange = onPlayerInRange;
            return this;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _data = state.EnemyData;
            _transform = state.transform;
            state.SetTimer(CHASE_ATTACK_TIMER_ID);
            state.SetTimer(CHASE_MOVE_TIMER_ID);
        }

        public override void Execute(EnemyLogic state)
        {
            _transform.position = Vector2.MoveTowards(_transform.position, PlayerPos, (float)state.CheckTimer(CHASE_MOVE_TIMER_ID) * state.EnemyData.MovementSpeed);
            state.SetTimer(CHASE_MOVE_TIMER_ID);
            if (Vector2.Distance(_transform.position, PlayerPos) > _range) return;

            if (state.CheckTimer(CHASE_ATTACK_TIMER_ID) < 1f / _data.AttackSpeed) return;
            state.SetTimer(CHASE_ATTACK_TIMER_ID);

            if (_onPlayerInRange != null) _onPlayerInRange.Invoke(state);
            else PlayerManager.Instance.PlayerHealth.GetDamaged(_data.Damage);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}