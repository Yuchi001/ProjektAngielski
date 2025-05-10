using EnemyPack.SO;
using EnemyPack.States.StateData;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;

namespace EnemyPack.States
{
    public class ShootState : StateBase
    {
        private readonly ShootStateData _data;
        private StateBase _lastState;

        private float AttackSpeed => 1f / _data.AttackRate;

        private float _timer;
        
        public ShootState(SoEnemy data) : base(data)
        {
            _data = data.GetStateData<ShootStateData>();
            _timer = -1;
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _lastState = lastState;
            if (_timer < 0) _timer = AttackSpeed;
        }

        public override void Execute(EnemyLogic state)
        {
            _lastState.Execute(state);

            if (!InRange(state, _data.AttackRange))
            {
                state.SwitchState(_lastState);
                return;
            }
            
            _timer += state.fixedDeltaTime;
            if (_timer < AttackSpeed) return;

            _timer = 0;
            var spawnPos = state.transform.position;
            var movementStrategy = new DirectionMovementStrategy(spawnPos, PlayerManager.PlayerPos, _data.BulletSpeed);
            var projectile = ProjectileManager.SpawnProjectile(movementStrategy, _data.Damage, spawnPos, ProjectileManager.PLAYER_TAG);
            projectile
                .SetRange(_data.AttackRange)
                .SetSprite(_data.BulletSprites, _data.BulletAnimationRate)
                .SetScale(_data.BulletScale)
                .Ready();
        }

        public override void Reset(EnemyLogic state)
        {
            _timer = -1;
        }
    }
}