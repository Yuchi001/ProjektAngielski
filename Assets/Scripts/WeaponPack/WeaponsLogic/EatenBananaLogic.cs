using EnemyPack;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class EatenBananaLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject boomParticles;

        private float BlastRange => GetStatValue(EWeaponStat.BlastRange) ?? 0;
        
        protected override void UseWeapon()
        {
            var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();
                
            projectileScript.Setup(Damage, 0)
                .SetSprite(projectileSprite)
                .SetScale(0.5f)
                .SetOnHitAction(OnHit)
                .SetOnHitParticles(boomParticles, BlastRange * 10)
                .SetLightColor(Color.red)
                .SetReady();
        }

        private void OnHit(GameObject hitObj, Projectile projectile)
        {
            var hitObjs = Physics2D.OverlapCircleAll(projectile.transform.position, BlastRange);

            foreach (var hit in hitObjs)
            {
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(Damage);
            }
        }
    }
}