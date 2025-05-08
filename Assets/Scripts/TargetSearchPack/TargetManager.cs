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
        public static bool TryFindViableTargets(FindTargetStrategyBase strategyBase, ref List<EnemyLogic> targets, float range, List<int> usedTargets = null)
        {
            var found = EnemyManager.GetNearbyEnemies(strategyBase.Center, range, ref targets);
            if (!found || usedTargets == null) return found;
            
            for (var i = targets.Count - 1; i > 0; i--)
            {
                if (!usedTargets.Contains(targets[i].GetInstanceID())) continue;
                targets.RemoveAt(i);
            }
            return targets.Count > 0;
        }
    }
}