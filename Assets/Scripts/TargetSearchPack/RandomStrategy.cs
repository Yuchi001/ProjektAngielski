using System.Collections.Generic;
using EnemyPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public class RandomStrategy : FindTargetStrategyBase
    {
        public override EnemyLogic FindEnemy(List<PoolObject> enemies)
        {
            return enemies.RandomElement().As<EnemyLogic>();
        }
    }
}