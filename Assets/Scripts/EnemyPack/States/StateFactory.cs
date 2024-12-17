using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.States.BatSmall;
using EnemyPack.States.Mimic;

namespace EnemyPack.States
{
    public static class StateFactory
    {
        private static readonly Dictionary<EEnemyBehaviour, Func<StateBase>> _states = new()
        {
            { EEnemyBehaviour.BatSmall, () => new BatSmallMain() },
            { EEnemyBehaviour.Mimic, () => new MimicMain() }
        };

        public static StateBase GetState(EEnemyBehaviour type)
        {
            return _states[type].Invoke();
        }
    }
}