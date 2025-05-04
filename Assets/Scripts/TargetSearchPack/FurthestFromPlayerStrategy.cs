using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public class FurthestFromPlayerStrategy : FindTargetStrategyBase
    {
        public override EnemyLogic FindEnemy(List<EnemyLogic> enemies)
        {
            (EnemyLogic enemy, float distance) farthest = (enemies[0], 0);
            foreach (var enemy in enemies)
            {
                var distance = enemy.transform.Distance(PlayerManager.PlayerPos);
                if (distance < farthest.distance) continue;
                farthest = (enemy, distance);
            }

            return farthest.enemy;
        }
    }
}