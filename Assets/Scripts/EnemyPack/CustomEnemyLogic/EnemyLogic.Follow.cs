using UnityEngine;

namespace EnemyPack.CustomEnemyLogic
{
    public partial class EnemyLogic
    {
        private void UpdateFollowBehaviour()
        {
            if (_target == null) return;
            
            bodyTransform.rotation = Quaternion.Euler(0, _target.position.x < transform.position.x ? 0 : 180, 0);

            var dir = _target.position - transform.position;
            dir.Normalize();
            _desiredDir = dir;
        }
        
        private void FixedUpdateFollowBehaviour()
        {
            if (_target == null) return;
            rb2d.velocity = _desiredDir * (GetMovementSpeed() * 1.5f);
        }
    }
}