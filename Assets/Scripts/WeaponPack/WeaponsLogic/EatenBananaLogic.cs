using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Managers.Enums;
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
        
        protected override bool UseWeapon()
        {
            var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(Damage, 0)
                .SetSprite(projectileSprite)
                .SetScale(0.5f)
                .SetOnHitAction(OnHit)
                .SetOnHitParticles(boomParticles, BlastRange * 2)
                .SetLightColor(Color.yellow)
                .SetReady();

            return true;
        }

        private void OnHit(GameObject hitObj, Projectile projectile)
        {
            AudioManager.Instance.PlaySound(ESoundType.BananaBoom);
            
            var hitObjs = Physics2D.OverlapCircleAll(projectile.transform.position, BlastRange);

            foreach (var hit in hitObjs)
            {
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(Damage);
            }
        }
    }
}