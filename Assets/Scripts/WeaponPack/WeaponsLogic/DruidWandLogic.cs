using EnemyPack;
using PlayerPack;
using UnityEngine;
using WeaponPack.Enums;

namespace WeaponPack.WeaponsLogic
{
    public class DruidWandLogic : WeaponLogicBase
    {
        private float Range => GetStatValue(EWeaponStat.BlastRange) ?? 0;
        private float HealValue => GetStatValue(EWeaponStat.HealValue) ?? 0;
        
        protected override bool UseWeapon()
        {
            var colliders = Physics2D.OverlapCircleAll(PlayerPos, Range);
            foreach (var collider in colliders)
            {
                if(!collider.TryGetComponent(out EnemyLogic enemyLogic)) continue;

                var diff = enemyLogic.transform.position - (Vector3)PlayerPos;
                diff = diff.normalized;
                enemyLogic.PushEnemy(diff * PushForce, 0.3f);
            }
            
            PlayerManager.Instance.PlayerHealth.Heal((int)HealValue);

            return true;
        }
    }
}