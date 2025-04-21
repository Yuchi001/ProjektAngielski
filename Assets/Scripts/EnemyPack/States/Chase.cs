using System;
using EnemyPack.SO;
using PlayerPack;
using UnityEngine;
using WorldGenerationPack;

namespace EnemyPack.States
{
    public class Chase : StateBase
    {
        private float _timer = 0;
        private readonly float? _attackRange = null;
        private float? _maxRange = null;

        private readonly Action<EnemyLogic> _onPlayerInRange = null;
        private  Action<EnemyLogic> _onPlayerOutOfRange = null;
        private SoEnemy _data;
        private Transform _transform;
        
        public Chase()
        {
            _onPlayerInRange = null;
        }

        public Chase(Action<EnemyLogic> onPlayerInRange, float? attackRange = null)
        {
            _onPlayerInRange = onPlayerInRange;
            _attackRange = attackRange;
        }

        public Chase SetOutOfRangeAction(Action<EnemyLogic> onPlayerOutOfRange, float maxRange)
        {
            _onPlayerOutOfRange = onPlayerOutOfRange;
            _maxRange = maxRange;
            return this;
        }
        
        public override void Enter(EnemyLogic state)
        {
            _data = state.EnemyData;
            _transform = state.transform;
            _timer = 0;
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            var position = _transform.position;
            var direction = (PlayerPos - (Vector2)position).normalized;

            var separation = Vector2.zero;
            foreach (var poolObj in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == _transform.gameObject) continue;
                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            _transform.position += (Vector3)(finalDir * (deltaTime * state.EnemyData.MovementSpeed));

            if (!InRange(state, _maxRange) && _onPlayerOutOfRange != null)
            {
                _onPlayerOutOfRange.Invoke(state);
                return;
            }
            
            if (!InRange(state, _attackRange)) return;

            if (_onPlayerInRange != null)
            {
                _onPlayerInRange.Invoke(state);
                return;
            }

            _timer += deltaTime;
            if (_timer < 1f / _data.AttackSpeed) return;
            _timer = 0;

            PlayerManager.PlayerHealth.GetDamaged(_data.Damage);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}