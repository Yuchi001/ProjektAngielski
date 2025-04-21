using EnemyPack.Enums;
using PlayerPack;
using UnityEngine;

namespace EnemyPack.States.RootStates
{
    public class MimicMain : StateBase, IRootState
    {
        public override bool CanBeStunned => false;
        public override bool CanBePushed => false;
        
        private const float DETECTION_RANGE = 1f;

        private Transform _transform;

        private new static Vector2 PlayerPos => PlayerManager.PlayerPos;
        
        public override void Enter(EnemyLogic state)
        {
            state.SetInvincible(true);
            _transform = state.transform;
            state.Animator.enabled = false;
            state.SpriteRenderer.flipX = false;

            var sprite = state.EnemyData.EnemySprite;
            state.SpriteRenderer.sprite = sprite;
        }

        public override void Execute(EnemyLogic state, float deltaTime)
        {
            if (Vector2.Distance(_transform.position, PlayerPos) > DETECTION_RANGE) return;

            state.Animator.enabled = true;
            state.Animator.Play("Idle");
            
            state.SwitchState(new Chase());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }

        public override ESpriteRotation GetRotation(EnemyLogic state)
        {
            return ESpriteRotation.None;
        }

        public void Compose(EnemyLogic logic)
        {
            
        }
    }
}