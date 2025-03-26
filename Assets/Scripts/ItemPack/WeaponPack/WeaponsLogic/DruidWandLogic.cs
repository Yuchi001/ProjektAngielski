using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class DruidWandLogic : ItemLogicBase
    {
        private float Range => GetStatValue(EItemSelfStatType.BlastRange) ?? 0;
        private float HealValue => GetStatValue(EItemSelfStatType.HealValue) ?? 0;
        
        protected override bool Use()
        {
            var results = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(PlayerPos, Range, results);
            foreach (var hit in results)
            {
                if(hit == null || !hit.TryGetComponent(out EnemyLogic enemyLogic)) continue;

                var diff = enemyLogic.transform.position - (Vector3)PlayerPos;
                diff = diff.normalized;
                enemyLogic.PushEnemy(diff * PushForce, 0.3f);
            }
            
            PlayerManager.Instance.PlayerHealth.Heal((int)HealValue);

            return true;
        }
    }
}