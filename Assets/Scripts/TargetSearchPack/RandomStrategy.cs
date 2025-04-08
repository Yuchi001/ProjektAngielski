using System.Collections.Generic;
using EnemyPack.CustomEnemyLogic;
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