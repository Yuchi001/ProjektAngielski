using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.States.RootStates;

namespace EnemyPack.States
{
    public static class StateFactory
    {
        private static readonly Dictionary<EEnemyBehaviour, Func<IRootState>> _states = new()
        {
            { EEnemyBehaviour.BatSmall, () => new BatSmallMain() },
            { EEnemyBehaviour.Mimic, () => new MimicMain() },
            { EEnemyBehaviour.Golem, () => new GolemMain() },
            { EEnemyBehaviour.BatSmallShoot, () => new BatSmallShootMain() },
            { EEnemyBehaviour.Monument, () => new MonumentMain() }
        };

        public static IRootState GetState(EEnemyBehaviour type)
        {
            return _states[type].Invoke();
        }
    }
}