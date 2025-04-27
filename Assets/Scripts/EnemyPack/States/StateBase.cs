using System;
using EnemyPack.Enums;
using EnemyPack.SO;
using Other;
using PlayerPack;
using UnityEngine;
using Utils;

namespace EnemyPack.States
{
    public abstract class StateBase
    {
        public virtual bool CanBeStunned { get; protected set; } = true;
        public virtual bool CanBePushed { get; protected set; } = true;

        protected static Vector2 PlayerPos => PlayerManager.PlayerPos;
        
        public abstract void Enter(EnemyLogic state, StateBase lastState);
        public abstract void Execute(EnemyLogic state);

        public abstract void Reset(EnemyLogic state);

        public virtual ESpriteRotation GetRotation(EnemyLogic state)
        {
            return state.EnemyData.SpriteRotation;
        }
        
        public StateBase(SoEnemy data)
        {
            
        }

        public T SetCanBePushed<T>(bool canBePushed) where T : StateBase
        {
            CanBePushed = canBePushed;
            return (T)this;
        }
        
        public T SetCanBeStunned<T>(bool canBeStunned) where T : StateBase
        {
            CanBeStunned = canBeStunned;
            return (T)this;
        }
        
        protected bool InRange(CanBeDamaged state, float? range = null)
        {
            var playerRadius = PlayerManager.PlayerHealth.BodyRadius;
            var enemyRadius = state.BodyRadius * state.transform.localScale.x;
            range ??= enemyRadius;
            var totalRange = 0.1f + playerRadius + range.Value;
            var totalRangeSqr = totalRange * totalRange;

            return state.transform.InRange(PlayerPos, totalRangeSqr);
        }
    }
}