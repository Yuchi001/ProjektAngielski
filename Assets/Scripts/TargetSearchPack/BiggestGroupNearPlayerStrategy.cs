using System.Collections.Generic;
using EnemyPack;
using PlayerPack;
using PoolPack;
using UnityEngine;

namespace TargetSearchPack
{
    public class BiggestGroupNearPlayerStrategy : FindTargetStrategyBase
    {
        private readonly FindTargetStrategyBase _pickInGroup;
        
        public BiggestGroupNearPlayerStrategy(FindTargetStrategyBase pickInGroupStrategyBase)
        {
            _pickInGroup = pickInGroupStrategyBase;
        }

        public override EnemyLogic FindEnemy(List<PoolObject> enemies)
        {
            var left = new List<PoolObject>();
            var right = new List<PoolObject>();
            foreach (var enemy in enemies)
            {
                if (enemy.transform.position.x > Center.x) right.Add(enemy);
                else left.Add(enemy);
            }

            return _pickInGroup.FindEnemy(right.Count > left.Count ? right : left);
        }
    }
}