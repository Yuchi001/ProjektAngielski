using System;
using System.Collections.Generic;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class BatSmallShootMain : RootStateBase
    {

        protected override StateBase GoToState => _patrolState;
        private PatrolState _patrolState;
        
        public override void Compose(EnemyLogic logic)
        {
            var baseData = logic.EnemyData.GetStateData<BatSmallShootData>();
            var shootStateData = logic.EnemyData.GetStateData<ShootStateData>();
            _patrolState = new PatrolState(baseData.PatrolRange, enemyLogic => shootStateData.Shoot(enemyLogic)).SetOnPlayerInRange(baseData.DetectionRange, enemyLogic => enemyLogic.SwitchState(_patrolState));
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(BatSmallShootData),
            typeof(ShootStateData)
        };

        [System.Serializable]
        public class BatSmallShootData : StateDataBase
        {
            [SerializeField] private float patrolRange;
            [SerializeField] private float detectionRange;

            public float PatrolRange => patrolRange;
            public float DetectionRange => detectionRange;
        }
    }
}