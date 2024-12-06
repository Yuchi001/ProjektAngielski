using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic.Base
{
    public abstract class FireArmBase : ItemLogicBase
    {
        [SerializeField] protected float bulletScale = 0.2f;
        [SerializeField] protected float trailTime = 0.2f;
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected Sprite projectileSprite;
        
        private float Accuracy => GetStatValue(EItemSelfStatType.Accuracy) ?? 1;
        
        /// <summary>
        /// Base for fire arm bullet, options already set:
        ///<br/> - light color
        ///<br/> - scale
        ///<br/> - sprite
        ///<br/> - push force
        ///<br/> - trail
        ///<br/> - on hit action
        /// </summary>
        /// <returns></returns>
        protected Projectile SpawnProjectile(Vector2 position)
        {
            var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(Damage, Speed)
                .SetDirection(GetDirection(position))
                .SetSprite(projectileSprite)
                .SetScale(bulletScale)
                .SetTrail(trailTime)
                .SetPushForce(PushForce);
            
            if (PlayerEnchantments.Has(EEnchantmentName.ExplosiveBullets))
                projectileScript.SetOnHitAction(OnHitAction);

            return projectileScript;
        }

        private void OnHitAction(GameObject hitObj, Projectile projectile)
        {
            var position = hitObj.transform.position;
            var results = new Collider2D[50];
            var range = PlayerEnchantments.GetParamValue(EEnchantmentName.ExplosiveBullets, EValueKey.Range);
            Physics2D.OverlapCircleNonAlloc(position, range, results);

            AudioManager.Instance.PlaySound(ESoundType.BulletExplode);
            SpecialEffectManager.Instance.SpawnExplosion(ESpecialEffectType.ExplosionMedium, position, range);

            var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.ExplosiveBullets, EValueKey.Percentage);
            var damage = Mathf.CeilToInt(Damage * percentage);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(damage);
            }
        }
        
        private Vector2 GetDirection(Vector2 pickedTargetPos)
        {
            var pos = pickedTargetPos;
            pos.x += Random.Range(-Accuracy, Accuracy);
            pos.y += Random.Range(-Accuracy, Accuracy);
            return pos;
        }
    }
}