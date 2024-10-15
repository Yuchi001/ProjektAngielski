using System.Collections.Generic;
using EnchantmentPack.Enums;
using Managers;
using Other.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class AxeLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;
        [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float rotationSpeedModifier;

        private float ProjectileScale => GetStatValue(EWeaponStat.ProjectileScale) ?? 0;
        
        protected override bool UseWeapon()
        {
            var targetedEnemies = new List<int>();
            var spawnedProjectiles = 0;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var target = UtilsMethods.FindTarget(transform.position, targetedEnemies);
                if (target == null) continue;

                spawnedProjectiles++;
                
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                targetedEnemies.Add(target.GetInstanceID());
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(target.transform.position)
                    .SetDontDestroyOnHit()
                    .SetSprite(projectileSprite)
                    .SetPushForce(PushForce)
                    .SetScale(ProjectileScale)
                    .SetRotationSpeed(-rotationSpeedModifier * Speed)
                    .SetLightColor(Color.clear);

                if (PlayerEnchantmentManager.Has(EEnchantmentName.Sharpness))
                {
                    var parameters = GameManager.Instance.EnchantmentValueDictionary[EEnchantmentName.Sharpness];
                    if (Random.Range(0f, 1f) <= parameters[EValueKey.Percentage]) projectileScript.SetEffect(EEffectType.Bleed, 9999);
                }
                
                projectileScript.SetReady();
            }

            return spawnedProjectiles > 0;
        }
    }
}