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

        private readonly List<EnemyLogic> _leftCachedTargets = new();
        private readonly List<EnemyLogic> _rightCachedTargets = new();
        
        public BiggestGroupNearPlayerStrategy(FindTargetStrategyBase pickInGroupStrategyBase)
        {
            _pickInGroup = pickInGroupStrategyBase;
        }

        public override EnemyLogic FindEnemy(List<EnemyLogic> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.transform.position.x > Center.x) _rightCachedTargets.Add(enemy);
                else _leftCachedTargets.Add(enemy);
            }
            
            var target = _pickInGroup.FindEnemy(_rightCachedTargets.Count > _leftCachedTargets.Count ? _rightCachedTargets : _leftCachedTargets);
            
            _rightCachedTargets.Clear();
            _leftCachedTargets.Clear();

            return target;
        }
    }
}