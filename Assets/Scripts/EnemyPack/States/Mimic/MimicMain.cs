using EnemyPack.CustomEnemyLogic;
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
        }

        public override void Execute(EnemyLogic state)
        {
            if (Vector2.Distance(_transform.position, PlayerPos) > DETECTION_RANGE) return;
            
            state.Animator.Play("Idle");
            state.SwitchState(new Chase());
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}