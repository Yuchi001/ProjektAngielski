using System;
using System.Collections.Generic;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class BatSmallMain : RootStateBase
    {
        protected override StateBase GoToState => _patrolState;

        private ChaseState _chaseState;
        private PatrolState _patrolState;
        
        public override void Compose(EnemyLogic logic)
        {
            var data = logic.EnemyData.GetStateData<BatSmallData>();
            _chaseState = new ChaseState().SetOutOfRangeAction(enemyLogic => enemyLogic.SwitchState(_patrolState), data.IdleRange);
            _patrolState = new PatrolState(data.PatrolRange).SetOnPlayerInRange(data.DetectionRange, enemyLogic => enemyLogic.SwitchState(_chaseState));
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(BatSmallData)
        };

        [System.Serializable]
        public class BatSmallData : StateDataBase
        {
            [SerializeField] private float idleRange;
            [SerializeField] private float patrolRange;
            [SerializeField] private float detectionRange;

            public float IdleRange => idleRange;
            public float PatrolRange => patrolRange;
            public float DetectionRange => detectionRange;
        }
    }
}