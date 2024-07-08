using UnityEngine;

namespace EnemyPack.EnemyBehaviours
{
    public abstract class EnemyBehaviourBase : MonoBehaviour
    {
        protected EnemyLogic EnemyLogic;

        public virtual void Setup(EnemyLogic enemyLogic)
        {
            EnemyLogic = enemyLogic;
        }

        public abstract void OnUpdate();

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnDie()
        {
        }
    }
}