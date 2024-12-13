using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.States.BatSmall;

namespace EnemyPack.States
{
    public static class StateFactory
    {
        private static readonly Dictionary<EEnemyBehaviour, Func<StateBase>> _states = new()
        {
            { EEnemyBehaviour.BatSmall, () => new BatSmallMain() }
        };

        public static StateBase GetState(EEnemyBehaviour type)
        {
            return _states[type].Invoke();
        }
    }
}