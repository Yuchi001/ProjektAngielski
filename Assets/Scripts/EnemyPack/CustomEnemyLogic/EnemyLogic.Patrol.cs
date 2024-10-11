using Managers;
using UnityEngine;

namespace EnemyPack.CustomEnemyLogic
{
    public partial class EnemyLogic
    {
        private void UpdatePatrolBehaviour()
        {
            var currentPos = (Vector2)transform.position;
            if (Vector2.Distance(currentPos, _desiredPos) <= 0.1f)
            {
                _desiredPos = GameManager.Instance.MapGenerator.GetRandomPos();
            }
            
            var dir = _desiredPos - currentPos;
            dir.Normalize();
            _desiredDir = dir;
        }
        
        private void FixedUpdatePatrolBehaviour()
        {
            rb2d.velocity = _desiredDir * MovementSpeed;
        }
    }
}