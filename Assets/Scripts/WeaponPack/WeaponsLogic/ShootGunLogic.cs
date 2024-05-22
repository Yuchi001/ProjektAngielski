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
    public class ShootGunLogic : WeaponLogicBase
    {
        [SerializeField] private float timeBetweenShoots = 0.1f;
        [SerializeField] private float maxGrainTimeBreak = 0.01f;
        [SerializeField] private float bulletScale = 0.2f;
        [SerializeField] private float trailTime = 0.2f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Sprite projectileSprite;

        private float Accuracy => GetStatValue(EWeaponStat.Accuracy) ?? 1;
        
        protected override void UseWeapon()
        {
            var target = UtilsMethods.FindTarget(transform.position);

            if (target == null) return;
            var position = target.transform.position;
            StartCoroutine(ShootAllMagazines(position));
        }

        private IEnumerator ShootAllMagazines(Vector2 position)
        {
            for (var j = 0; j < 2; j++)
            {
                AudioManager.Instance.PlaySound(ESoundType.PistolShoot);
                for (var i = 0; i < ProjectileCount; i++)
                {
                    var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                    var projectileScript = projectile.GetComponent<Projectile>();
                
                    projectileScript.Setup(Damage, Speed)
                        .SetDirection(GetDirection(position))
                        .SetLightColor(Color.clear)
                        .SetSprite(projectileSprite)
                        .SetScale(bulletScale)
                        .SetTrail(trailTime)
                        .SetDontDestroyOnHit()
                        .SetPushForce(PushForce)
                        .SetReady();

                    var waitTime = Random.Range(0.001f, maxGrainTimeBreak + 0.001f);
                
                    yield return new WaitForSeconds(waitTime);
                }

                yield return new WaitForSeconds(timeBetweenShoots);
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