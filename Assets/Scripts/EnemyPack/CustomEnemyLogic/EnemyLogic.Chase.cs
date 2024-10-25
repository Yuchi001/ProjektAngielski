
using Managers;
using Other.Enums;
using UnityEngine;

namespace EnemyPack.CustomEnemyLogic
{
    public partial class EnemyLogic
    {
        [Header("Chase")] 
        [SerializeField] private float stunDuration;
        [SerializeField] private float maxChaseSpeed = 3.25f;

        private float RawChaseSpeed =>
            _playerSpeed - _enemy.MovementSpeed >= maxChaseSpeed ? maxChaseSpeed : _playerSpeed - _enemy.MovementSpeed;
        private float ChaseSpeed => Slowed ? RawChaseSpeed / 2f : RawChaseSpeed;
        
        private void UpdateChaseBehaviour()
        {
            if (_target == null) return;
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            var dir = _target.position - transform.position;
            dir.Normalize();
            _desiredDir = dir;
        }
        
        private void FixedUpdateChaseBehaviour()
        {
            if (_target == null) return;
            rb2d.velocity = _desiredDir * (GetMovementSpeed() * 1.5f);
        }

        private void PlayerHitChaseBehaviour()
        {
            var effectInfo = new EffectInfo
            {
                effectType = EEffectType.Stun,
                time = stunDuration,
            };
            AddEffect(effectInfo);
        }
    }
}