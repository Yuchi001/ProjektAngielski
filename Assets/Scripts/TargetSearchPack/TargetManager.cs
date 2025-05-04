using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using PoolPack;
using Utils;
using WorldGenerationPack;

namespace TargetSearchPack
{
    public static class TargetManager
    {
        public static EnemyLogic FindTarget(FindTargetStrategyBase strategyBase, float range, List<int> usedTargets = null)
        {
            var found = TryFindViableEnemies(strategyBase, out var enemies, range, usedTargets);
            return found ? strategyBase.FindEnemy(enemies) : null;
        }

        public static bool TryFindViableEnemies(FindTargetStrategyBase strategyBase, out List<EnemyLogic> targets, float range, List<int> usedTargets = null)
        {
            targets = new List<EnemyLogic>();
            var found = EnemyManager.GetNearbyEnemies(strategyBase.Center, range, ref targets);
            if (!found) return false;
            if (usedTargets != null) targets = targets.Where(e => !usedTargets.Contains(e.GetInstanceID())).ToList();
            return targets.Count > 0;
        }
    }
}