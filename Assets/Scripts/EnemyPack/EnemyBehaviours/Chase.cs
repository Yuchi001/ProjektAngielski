using PlayerPack;
using UnityEngine;

namespace EnemyPack.EnemyBehaviours
{
    public partial class EnemyLogic
    {
        public class Chase : EnemyBehaviourBase
        {
            private Transform _target;

            public override void Setup(EnemyLogic enemyLogic)
            {
                _target = PlayerManager.Instance.transform;
            }

            public override void OnUpdate()
            {
                
            }
        }
    }
}