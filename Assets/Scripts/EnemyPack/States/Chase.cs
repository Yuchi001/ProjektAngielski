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

        private new static Vector2 PlayerPos => PlayerManager.PlayerPos;

        public Chase(StateBase nextState, bool runAwa)
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
            var transform = state.transform;
            var position = transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var separation = Vector2.zero;
            var nearby = Physics2D.OverlapCircleAll(position, 1);
            foreach (var col in nearby)
            {
                if (col.gameObject == transform.gameObject || !col.CompareTag("Enemy")) continue;
                var pushAway = (Vector2)(transform.position - col.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            transform.position += (Vector3)(finalDir * ((float)state.CheckTimer(CHASE_MOVE_TIMER_ID) * state.EnemyData.MovementSpeed));
            
            state.SetTimer(CHASE_MOVE_TIMER_ID);
            if (Vector2.Distance(_transform.position, PlayerPos) > _range) return;

            if (state.CheckTimer(CHASE_ATTACK_TIMER_ID) < 1f / _data.AttackSpeed) return;
            state.SetTimer(CHASE_ATTACK_TIMER_ID);

            if (_onPlayerInRange != null) _onPlayerInRange.Invoke(state);
            else PlayerManager.PlayerHealth.GetDamaged(_data.Damage);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}