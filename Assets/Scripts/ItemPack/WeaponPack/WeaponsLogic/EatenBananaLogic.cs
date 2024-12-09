using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class EatenBananaLogic : ItemLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject boomParticles;

        private float BlastRange => GetStatValue(EItemSelfStatType.BlastRange) ?? 0;
        
        protected override bool Use()
        {
            var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);

            projectile.Setup(Damage, 0)
                .SetSprite(projectileSprite)
                .SetScale(0.5f)
                .SetOnHitAction(OnHit)
                .SetOnHitParticles(boomParticles, BlastRange * 2)
                .SetReady();

            return true;
        }

        private void OnHit(GameObject hitObj, Projectile projectile)
        {
            AudioManager.Instance.PlaySound(ESoundType.BananaBoom);

            var projectilePos = projectile.transform.position;
            SpecialEffectManager.Instance.SpawnExplosion(ESpecialEffectType.ExplosionBig, projectilePos, BlastRange);

            var results = new Collider2D[50];
            Physics2D.OverlapCircleNonAlloc(projectilePos, BlastRange, results);

            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(Damage);
            }
        }
    }
}