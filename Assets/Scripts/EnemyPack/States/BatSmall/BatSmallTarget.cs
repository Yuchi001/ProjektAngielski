using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using UnityEngine;

namespace EnemyPack.States.BatSmall
{
    public class BatSmallTarget : StateBase
    {
        public override bool CanBeStuned => true;
        public override bool CanBePushed => true;

        private static Projectile ProjectilePrefab => GameManager.Instance.ProjectilePrefab;
        private Transform _transform;
        private SoEnemy _enemyData;

        private const string SHOOT_TIMER_ID = "BAT_SMALL_SHOOT_TIMER";
        private const float SHOOT_RATE_PER_SEC = 2f;
        private const float BULLET_SPEED = 2f;
        
        public override void Enter(EnemyLogic state)
        {
            _transform = state.transform;
            _enemyData = state.EnemyData;
            state.SetTimer(SHOOT_TIMER_ID);
        }

        public override void Execute(EnemyLogic state)
        {
            if (state.CheckTimer(SHOOT_TIMER_ID) < 1f / SHOOT_RATE_PER_SEC) return;

            var spawnedBullet = Object.Instantiate(ProjectilePrefab, _transform.position, Quaternion.identity)
                .Setup(_enemyData.Damage, BULLET_SPEED);
            
            spawnedBullet.SetReady();
        }

        public override void Reset(EnemyLogic state)
        {
            
        }
    }
}