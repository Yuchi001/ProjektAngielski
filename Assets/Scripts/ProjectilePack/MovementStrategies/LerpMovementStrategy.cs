using UnityEngine;

namespace ProjectilePack.MovementStrategies
{
    public class LerpMovementStrategy : IProjectileMovementStrategy
    {
        private readonly float _time;
        private readonly Vector2 _targetPosition;
        private readonly Vector2 _startPosition;

        private float _timer = 0;
        
        public LerpMovementStrategy(Vector2 startPosition, Vector2 targetPosition, float time)
        {
            _time = time;
            _targetPosition = targetPosition;
            _startPosition = startPosition;
        }
        
        public void MoveProjectile(Projectile projectile, float deltaTime)
        {
            _timer += deltaTime;
            projectile.transform.position = Vector2.Lerp(_startPosition, _targetPosition, _timer / _time);
        }
    }
}