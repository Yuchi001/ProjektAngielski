using System;
using System.Collections.Generic;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class MonumentMain : RootStateBase
    {
        protected override StateBase GoToState => _distanceTriggerState;
        private DistanceTriggerState _distanceTriggerState;
        
        private WaitingState _waitForTime;

        private ShootStateData _shootStateData;
        private MonumentData _monumentData;
        
        public override void Compose(EnemyLogic logic)
        {
            _monumentData = logic.EnemyData.GetStateData<MonumentData>();
            _shootStateData = logic.EnemyData.GetStateData<ShootStateData>();
            _waitForTime = new WaitingState(1f / _shootStateData.)
            
            ResetAnim(logic);
        }

        private void OnWaitOver(EnemyLogic state)
        {
            _waitForTime.Reset(state);
        }

        private void OnPlayerInRange(EnemyLogic state)
        {
            state.Animator.speed = 1f;
            _waitForTime.Execute(state);
        }

        private void OnPlayerExitRange(EnemyLogic state)
        {
            ResetAnim(state);
        }

        private void ResetAnim(EnemyLogic state)
        {
            var animator = state.Animator;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, 0f);
            animator.speed = 0f;
            state.SpriteRenderer.sprite = _monumentData.DisabledSprite;
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(MonumentData),
            typeof(ShootStateData)
        };

        [System.Serializable]
        public class MonumentData : StateDataBase
        {
            [SerializeField] private Sprite disabledSprite;

            public Sprite DisabledSprite => disabledSprite;
        }
    }
}