using System.Collections.Generic;
using System.Linq;
using AccessorPack;
using EnemyPack;
using PoolPack;
using Utils;

namespace TargetSearchPack
{
    public static class TargetManager
    {
        public static EnemyLogic FindTarget(FindTargetStrategyBase strategyBase, List<int> usedTargets = null, float? range = null)
        {
            var found = TryFindViableEnemies(strategyBase, out var enemies, usedTargets, range);
            return found ? strategyBase.FindEnemy(enemies) : null;
        }

        public static bool TryFindViableEnemies(FindTargetStrategyBase strategyBase, out List<PoolObject> targets, List<int> usedTargets = null, float? range = null)
        {
            targets = MainSceneAccessor.EnemySpawner.GetActiveEnemies();
            if (usedTargets != null) targets = targets.Where(e => !usedTargets.Contains(e.GetInstanceID())).ToList();
            if (range != null) targets = targets.Where(e => e.transform.Distance(strategyBase.Center) <= range).ToList();
            return targets.Count > 0;
        }
    }
}