using System.Collections.Generic;
using EnemyPack.CustomEnemyLogic;
using PlayerPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public class FurthestFromPlayerStrategy : FindTargetStrategyBase
    {
        public override EnemyLogic FindEnemy(List<PoolObject> enemies)
        {
            (PoolObject enemy, float distance) farthest = (enemies[0], 0);
            foreach (var enemy in enemies)
            {
                var distance = enemy.transform.Distance(PlayerManager.PlayerPos);
                if (distance < farthest.distance) continue;
                farthest = (enemy, distance);
            }

            return farthest.enemy.As<EnemyLogic>();
        }
    }
}