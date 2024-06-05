using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.Enums;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class PistolLogic : WeaponLogicBase
    {
        [SerializeField] private float bulletScale = 0.2f;
        [SerializeField] private float trailTime = 0.2f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;

        private float Accuracy => GetStatValue(EWeaponStat.Accuracy) ?? 1;
        
        protected override bool UseWeapon()
        {
            var target = UtilsMethods.FindTarget(transform.position);

            if (target == null) return false;
            var position = target.transform.position;
            StartCoroutine(ShootAllMagazines(position));
            return true;
        }

        private IEnumerator ShootAllMagazines(Vector2 position)
        {
            for (var i = 0; i < ProjectileCount; i++)
            {
                AudioManager.Instance.PlaySound(ESoundType.PistolShoot);
                
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Damage, Speed)
                    .SetDirection(GetDirection(position))
                    .SetLightColor(Color.clear)
                    .SetSprite(projectileSprite)
                    .SetScale(bulletScale)
                    .SetTrail(trailTime)
                    .SetPushForce(PushForce)
                    .SetReady();
                
                yield return new WaitForSeconds(Cooldown / (2 * ProjectileCount));
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