using EnemyPack.SO;
using UnityEngine;
using WorldGenerationPack;

namespace EnemyPack.States
{
    public class Flee : StateBase
    {
        private readonly float _minRange;

        private readonly StateBase _returnState;

        private SoEnemy _data;
        private Transform _transform;

        public Flee(StateBase returnState, float minRange)
        {
            _returnState = returnState;
            _minRange = minRange;
        }

        public override void Enter(EnemyLogic state)
        {
            _data = state.EnemyData;
            _transform = state.transform;
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            var position = _transform.position;
            var direction = ((Vector2)position - PlayerPos).normalized; // odwrotna niż Chase

            var separation = Vector2.zero;
            foreach (var poolObj in WorldGeneratorManager.EnemySpawner.GetActiveEnemies())
            {
                if (poolObj.gameObject == _transform.gameObject) continue;
                var pushAway = (Vector2)(_transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;
                if (dist > 0) separation += pushAway.normalized / dist;
            }

            var finalDir = (direction + separation * 0.1f).normalized;

            _transform.position += (Vector3)(finalDir * (deltaTime * _data.MovementSpeed));
            
            if (InRange(state, _minRange)) return;
            
            state.SwitchState(_returnState);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}