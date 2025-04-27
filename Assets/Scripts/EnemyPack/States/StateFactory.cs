using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.SO;
using EnemyPack.States.RootStates;

namespace EnemyPack.States
{
    public static class StateFactory
    {
        private static readonly Dictionary<EEnemyBehaviour, Func<SoEnemy, RootStateBase>> _states = new()
        {
            { EEnemyBehaviour.BatSmall, enemy => new BatSmallMain(enemy) },
            { EEnemyBehaviour.Mimic, enemy => new MimicMain(enemy) },
            { EEnemyBehaviour.Golem, enemy => new GolemMain(enemy) },
            { EEnemyBehaviour.BatSmallShoot, enemy => new BatSmallShootMain(enemy) },
            { EEnemyBehaviour.Monument, enemy => new MonumentMain(enemy) }
        };

        public static RootStateBase GetState(EEnemyBehaviour behaviour, SoEnemy data)
        {
            return _states[behaviour].Invoke(data);
        }
    }
}