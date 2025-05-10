using EnemyPack.SO;
using EnemyPack.States.StateData;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using UnityEngine;

namespace EnemyPack.States
{
    public class CircleShootState : StateBase
    {
        private readonly CircleShootStateData _data;
        private StateBase _lastState;
        
        private float AttackSpeed => 1f / _data.AttackRate;
        
        private float _timer = -1;
        
        public CircleShootState(SoEnemy data) : base(data)
        {
            _data = data.GetStateData<CircleShootStateData>();
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            if (_timer < 0) _timer = AttackSpeed;
            _lastState = lastState;
        }

        public override void Execute(EnemyLogic state)
        {
            _lastState.Execute(state);
            
            _timer += Time.deltaTime;
            if (_timer < AttackSpeed) return;

            _timer = 0;
            
            var angleStep = 360f / _data.BulletCount;

            for (var i = 0; i < _data.BulletCount; i++)
            {
                var angle = i * angleStep;
                var direction = AngleToDirection(angle);

                var movementStrategy = new DirectionMovementStrategy(direction, _data.BulletSpeed);

                var projectile = ProjectileManager.SpawnProjectile(movementStrategy, _data.Damage, state.transform.position, ProjectileManager.PLAYER_TAG);

                projectile
                    .SetRange(_data.AttackRange)
                    .SetScale(0.3f)
                    .SetSprite(_data.BulletSprites, _data.BulletAnimationRate)
                    .Ready();
            }

            state.SwitchState(_lastState);
        }

        public override void LazyExecute(EnemyLogic state, float lazyDeltaTime)
        {
            _lastState.LazyExecute(state, lazyDeltaTime);
            
            if (InRange(state, _data.AttackRange)) return;
            state.SwitchState(_lastState);
        }

        private static Vector2 AngleToDirection(float angleInDegrees)
        {
            float radians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }
    }
}