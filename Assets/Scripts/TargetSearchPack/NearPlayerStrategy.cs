using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public class NearPlayerStrategy : FindTargetStrategyBase
    {
        public override EnemyLogic FindEnemy(List<PoolObject> enemies)
        {
            (PoolObject enemy, float distance) closest = (enemies[0], 999f);
            foreach (var enemy in enemies)
            {
                var distance = enemy.transform.Distance(PlayerManager.PlayerPos);
                if (distance > closest.distance) continue;
                closest = (enemy, distance);
            }

            return closest.enemy.As<EnemyLogic>();
        }
    }
}