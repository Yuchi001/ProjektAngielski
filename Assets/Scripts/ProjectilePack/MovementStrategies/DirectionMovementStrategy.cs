using UnityEngine;

namespace ProjectilePack.MovementStrategies
{
    public class DirectionMovementStrategy : IProjectileMovementStrategy
    {
        private readonly float? _rotationSpeed;
        private readonly Vector2 _direction;
        private readonly float _speed;
        
        public DirectionMovementStrategy(Vector2 direction, float speed, float? rotationSpeed = null)
        {
            _rotationSpeed = rotationSpeed;
            _direction = direction;
            _speed = speed;
        }
        
        public DirectionMovementStrategy(Vector2 projectilePos, Vector2 targetPos, float speed, float? rotationSpeed = null)
        {
            _rotationSpeed = rotationSpeed;
            _direction = (targetPos - projectilePos).normalized;
            _speed = speed;
        }
        
        public void MoveProjectile(Projectile projectile)
        {
            if (_rotationSpeed.HasValue) projectile.SpriteRenderer.transform.Rotate(0, 0, _rotationSpeed.Value);
            projectile.transform.Translate(_direction * (_speed * projectile.DeltaTime));
        }
    }
}