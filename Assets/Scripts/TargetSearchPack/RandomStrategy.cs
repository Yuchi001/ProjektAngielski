using System.Collections.Generic;
using EnemyPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public class RandomStrategy : FindTargetStrategyBase
    {
        public override EnemyLogic FindEnemy(List<EnemyLogic> enemies)
        {
            return enemies.RandomElement();
        }
    }
}