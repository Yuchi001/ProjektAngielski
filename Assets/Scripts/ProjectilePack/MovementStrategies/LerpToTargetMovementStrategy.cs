using UnityEngine;

namespace ProjectilePack.MovementStrategies
{
    public class LerpToTargetMovementStrategy : IProjectileMovementStrategy
    {
        private readonly float _time;
        private readonly Transform _target;
        private readonly Vector2 _startPosition;

        private float _timer = 0;
        private Vector2 _lastTargetPos;
        
        public LerpToTargetMovementStrategy(Vector2 startPosition, Transform target, float time)
        {
            _time = time;
            _target = target;
            _startPosition = startPosition;
        }
        
        public void MoveProjectile(Projectile projectile, float deltaTime)
        {
            Vector2 position;
            if (_target != null) _lastTargetPos = position = _target.position;
            else position = _lastTargetPos;
            
            _timer += deltaTime;
            projectile.transform.position = Vector2.Lerp(_startPosition, position, _timer / _time);
        }
    }
}