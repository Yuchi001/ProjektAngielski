using EnchantmentPack.Enums;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class AxeLogic : ItemLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;
        [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float rotationSpeedModifier;

        private float ProjectileScale => GetStatValue(EItemSelfStatType.ProjectileScale) ?? 0;
        
        protected override bool Use()
        {
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindTargetInBiggestGroup(transform.position);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetSprite(projectileSprite)
                    .SetPushForce(PushForce)
                    .SetScale(ProjectileScale)
                    .SetRotationSpeed(-rotationSpeedModifier * Speed)
                    .SetLightColor(Color.clear);

                if (PlayerEnchantments.Has(EEnchantmentName.Sharpness))
                {
                    var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.Sharpness, EValueKey.Percentage);
                    if (Random.Range(0f, 1f) <= percentage) projectileScript.SetEffect(EEffectType.Bleed, 9999);
                }
                
                projectileScript.SetReady();
            }

            return spawnedProjectiles > 0;
        }
    }
}