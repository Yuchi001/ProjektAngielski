using System;
using EnemyPack.Enums;
using EnemyPack.SO;
using Other;
using Other.Enums;
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
        public abstract void LazyExecute(EnemyLogic state, float lazyDeltaTime);

        public virtual ESpriteRotation GetRotation(EnemyLogic state)
        {
            return state.EnemyData.SpriteRotation;
        }

        public virtual void Exit(EnemyLogic state)
        {
            
        }
        
        public StateBase(SoEnemy data)
        {
            
        }
        
        protected bool InRange(CanBeDamaged state, float? range = null)
        {
            var enemyRadius = state.BodyRadius * state.transform.localScale.x;
            range ??= enemyRadius;
            if (!PlayerManager.IsPlayerNearby(state.transform.position, range.Value)) return false;
            
            var playerRadius = PlayerManager.PlayerHealth.BodyRadius;
            var totalRange = 0.1f + playerRadius + range.Value;
            var totalRangeSqr = totalRange * totalRange;

            return state.transform.InRange(PlayerPos, totalRangeSqr);
        }

        protected float SlowModificator(EnemyLogic state) => state.HasEffect(EEffectType.Slow) ? 0.5f : 1f;
    }
}