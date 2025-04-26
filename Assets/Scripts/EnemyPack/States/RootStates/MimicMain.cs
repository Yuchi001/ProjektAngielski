using System;
using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class MimicMain : RootStateBase
    {
        protected override StateBase GoToState => _distanceTriggerState;
        private DistanceTriggerState _distanceTriggerState;

        public override bool CanBeStunned => false;
        public override bool CanBePushed => false;
        
        public override void Compose(EnemyLogic state)
        {
            state.SetInvincible(true);
            state.Animator.enabled = false;
            state.SpriteRenderer.flipX = false;

            var sprite = state.EnemyData.EnemySprite;
            state.SpriteRenderer.sprite = sprite;

            var data = state.EnemyData.GetStateData<MimicData>();
            _distanceTriggerState = new DistanceTriggerState(data.DetectionRange, OnPlayerEnter);
        }

        private void OnPlayerEnter(EnemyLogic state)
        {
            state.Animator.enabled = true;
            state.Animator.Play("Idle");
            
            state.SwitchState(new ChaseState());
        }

        public override ESpriteRotation GetRotation(EnemyLogic state)
        {
            return ESpriteRotation.None;
        }

        public override List<Type> RequiredDataTypes => new()
        {
            typeof(MimicData)
        };

        [System.Serializable]
        public class MimicData : StateDataBase
        {
            [SerializeField] private float detectionRange;

            public float DetectionRange => detectionRange;
        }
    }
}