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

        public override EnemyLogic FindEnemy(List<EnemyLogic> enemies)
        {
            var left = new List<EnemyLogic>();
            var right = new List<EnemyLogic>();
            foreach (var enemy in enemies)
            {
                if (enemy.transform.position.x > Center.x) right.Add(enemy);
                else left.Add(enemy);
            }

            return _pickInGroup.FindEnemy(right.Count > left.Count ? right : left);
        }
    }
}