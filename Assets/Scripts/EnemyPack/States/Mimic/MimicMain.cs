using EnemyPack.CustomEnemyLogic;
using EnemyPack.Enums;
using PlayerPack;
using UnityEngine;

namespace EnemyPack.States.Mimic
{
    public class MimicMain : StateBase
    {
        public override bool CanBeStunned => false;
        public override bool CanBePushed => false;
        
        private const float DETECTION_RANGE = 1f;

        private Transform _transform;

        private new static Vector2 PlayerPos => PlayerManager.Instance.PlayerPos;
        
        public override void Enter(EnemyLogic state)
        {
            state.SetInvincible(true);
            _transform = state.transform;
            state.Animator.enabled = false;
            state.Collider2D.isTrigger = true;
            state.SpriteRenderer.flipX = false;

            var sprite = state.EnemyData.EnemySprite;
            state.SpriteRenderer.sprite = sprite;
            _transform.localScale = Vector2.one * (sprite.pixelsPerUnit / 32 * state.EnemyData.BodyScale);
        }

        public override void Execute(EnemyLogic state)
        {
            if (Vector2.Distance(_transform.position, PlayerPos) > DETECTION_RANGE) return;

            _transform.localScale = Vector2.one * state.EnemyData.BodyScale; 
            state.Animator.enabled = true;
            state.Collider2D.isTrigger = false;
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
    }
}