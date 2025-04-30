using EnemyPack.Enums;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public class FreezeState : StateBase
    {
        private readonly FreezeStateData _data;
        
        public override bool CanBeStunned => false;
        public override bool CanBePushed => false;

        private Vector2 _startPos;

        public override ESpriteRotation GetRotation(EnemyLogic state)
        {
            return ESpriteRotation.None;
        }

        public FreezeState(SoEnemy data, FreezeStateData stateData) : base(data)
        {
            _data = stateData;
        }
        
        public FreezeState(SoEnemy data) : base(data)
        {
            _data = data.GetStateData<FreezeStateData>();
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            if (_data.UseFreezeSprite)
            {
                state.Animator.enabled = false;
                var sprite = _data.FreezeSprite;
                state.SpriteRenderer.sprite = sprite;
            }

            _startPos = state.transform.position;
            if (_data.Invincible) state.SetInvincible(true);
        }

        public override void Execute(EnemyLogic state)
        {
            state.transform.position = _startPos;
        }

        public override void Exit(EnemyLogic state)
        {
            if (_data.Invincible) state.SetInvincible(false);
            if (!_data.UseFreezeSprite) return;
            state.Animator.enabled = true;
            state.Animator.Play("Idle");
            state.SetInvincible(false);
        }

        public override void Reset(EnemyLogic state)
        {
            
        }

        protected void SwitchToNextState(EnemyLogic logic, StateBase state)
        {
            if (_data.UseFreezeSprite)
            {
                logic.Animator.enabled = true;
                logic.Animator.Play("Idle");
                logic.SetInvincible(false);
            }
            logic.SwitchState(state);
        }
    }
}