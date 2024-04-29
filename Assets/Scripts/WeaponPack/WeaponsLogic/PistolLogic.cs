using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class PistolLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;

        private float Accuracy => GetStatValue(EWeaponStat.Accuracy) ?? 1;
        private float BulletsPerSecond => GetStatValue(EWeaponStat.BulletsPerSecond) ?? 1;
        private float ProjectilesCount => GetStatValue(EWeaponStat.ProjectilesCount) ?? 1;
        
        protected override void UseWeapon()
        {
            var target = UtilsMethods.FindTarget(transform.position);

            if (target == null) return;
            StartCoroutine(ShootAllMagazines(target.transform.position));
        }

        private IEnumerator ShootAllMagazines(Vector2 position)
        {
            for (var i = 0; i < ProjectilesCount; i++)
            {
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(GetDirection(position))
                    .SetLightColor(Color.clear)
                    .SetSprite(projectileSprite)
                    .SetScale(0.5f)
                    .SetReady();
                
                yield return new WaitForSeconds(Cooldown / (2 * ProjectilesCount));
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