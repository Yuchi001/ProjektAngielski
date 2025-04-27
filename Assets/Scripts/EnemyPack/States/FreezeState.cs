using EnemyPack.SO;
using EnemyPack.States.StateData;
using UnityEngine;

namespace EnemyPack.States
{
    public abstract class FreezeState : StateBase
    {
        private readonly bool _isInvincible;
        
        public override bool CanBeStunned => false;
        public override bool CanBePushed => false;
        
        protected FreezeState(SoEnemy data, bool isInvincible) : base(data)
        {
            _isInvincible = isInvincible;
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            state.Animator.enabled = false;
            state.SpriteRenderer.flipX = false;

            var sprite = state.EnemyData.EnemySprite;
            state.SpriteRenderer.sprite = sprite;
            
            if (_isInvincible) state.SetInvincible(true);
        }

        protected static void SwitchToNextState(EnemyLogic logic, StateBase state)
        {
            logic.Animator.enabled = true;
            logic.Animator.Play("Idle");
            logic.SetInvincible(false);
            logic.SwitchState(state);
        }
    }
}