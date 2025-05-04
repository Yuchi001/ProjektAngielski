using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using PoolPack;
using UnityEngine;

namespace TargetSearchPack
{
    public abstract class FindTargetStrategyBase
    {
        public virtual Vector2 Center => PlayerManager.PlayerPos;
        
        public abstract EnemyLogic FindEnemy(List<EnemyLogic> enemies);
    }
}