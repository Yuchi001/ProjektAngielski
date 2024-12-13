using EnemyPack.CustomEnemyLogic;
using PlayerPack;
using UnityEngine;

namespace EnemyPack.States
{
    public abstract class StateBase
    {
        public abstract bool CanBeStuned { get; }
        public abstract bool CanBePushed { get; }

        protected Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;
        
        public abstract void Enter(EnemyLogic state);
        public abstract void Execute(EnemyLogic state);

        /// <summary>
        /// DONT CALL BASE METHOD
        /// </summary>
        /// <param name="state">current enemy logic reference</param>
        public virtual void FixedExecute(EnemyLogic state) { }

        public abstract void Reset(EnemyLogic state);
    }
}