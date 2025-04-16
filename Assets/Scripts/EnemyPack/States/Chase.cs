using System;
using AccessorPack;
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
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            var transform = state.transform;
            var position = transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var separation = Vector2.zero;
            foreach (var poolObj in MainSceneAccessor.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == transform.gameObject) continue;
                var pushAway = (Vector2)(transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            transform.position += (Vector3)(finalDir * (deltaTime * state.EnemyData.MovementSpeed));
            
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