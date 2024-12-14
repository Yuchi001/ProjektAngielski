using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using UnityEngine;
using Utils;

namespace EnemyPack.States.BatSmall
{
    public class BatSmallTarget : StateBase
    {
        public override bool CanBeStunned => true;
        public override bool CanBePushed => true;

        private static Projectile ProjectilePrefab => GameManager.Instance.ProjectilePrefab;
        private Transform _transform;
        private SoEnemy _enemyData;

        private const string SHOOT_TIMER_ID = "BAT_SMALL_SHOOT_TIMER";
        private const string MOVE_TIMER_ID = "BAT_SMALL_MOVE_ID";
        private const float SHOOT_RATE_PER_SEC = 2f;
        private const float BULLET_SPEED = 2f;

        public override void Enter(EnemyLogic state)
        {
            _transform = state.transform;
            _enemyData = state.EnemyData;
            state.SetTimer(SHOOT_TIMER_ID);
            state.SetTimer(MOVE_TIMER_ID);
        }

        public override void Execute(EnemyLogic state)
        {
            _transform.position = Vector2.MoveTowards(_transform.position, PlayerPos, (float)state.CheckTimer(MOVE_TIMER_ID) * state.EnemyData.MovementSpeed);
            state.SetTimer(MOVE_TIMER_ID);
            if (state.CheckTimer(SHOOT_TIMER_ID) < 1f / SHOOT_RATE_PER_SEC) return;

            state.SetTimer(SHOOT_TIMER_ID);
            return;
            Object.Instantiate(ProjectilePrefab, _transform.position, Quaternion.identity)
                .Setup(_enemyData.Damage, BULLET_SPEED)
                .SetDirection(PlayerPos)
                .SetTargetTag("Player", 7)
                .SetReady();
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}