using System;
using UnityEngine;
using Utils;

namespace ProjectilePack.MovementStrategies
{
    public class TargetMovementStrategy : IProjectileMovementStrategy
    {
        private Transform _target;
        private readonly float _speed;
        private readonly float? _rotationSpeed;
        private readonly Func<Projectile, bool> _onTargetNullAction;

        private Vector2 _currentDir;
        private Vector3 _lastPos;
        
        public TargetMovementStrategy(Transform target, float speed, Func<Projectile, bool> onTargetNullAction = null)
        {
            _target = target;
            _speed = speed;
            _onTargetNullAction = onTargetNullAction;
            _currentDir = (target.transform.position - _lastPos).normalized;
        }
        
        public TargetMovementStrategy(Transform target, float speed, float rotationSpeed, Func<Projectile, bool> onTargetNullAction = null)
        {
            _target = target;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _onTargetNullAction = onTargetNullAction;
            _currentDir = (target.transform.position - _lastPos).normalized;
        }
        
        public void MoveProjectile(Projectile projectile, float deltaTime)
        {
            if (_rotationSpeed.HasValue) projectile.SpriteRenderer.transform.Rotate(0, 0, _rotationSpeed.Value);
            
            var transform = projectile.transform;
            if (_target == null || !_target.gameObject.activeSelf)
            {
                _target = null;
                var shouldBreak = _onTargetNullAction?.Invoke(projectile) ?? false;
                if (shouldBreak) return;

                transform.MoveInDirection(_currentDir, _speed * deltaTime); // move at last dir in case target is null
                return;
            }

            var currentPos = transform.position;
            _currentDir = (currentPos - _lastPos).normalized;
            _lastPos = currentPos;
            projectile.transform.MoveTowards(_target.position, _speed * deltaTime);
        }
    }
}