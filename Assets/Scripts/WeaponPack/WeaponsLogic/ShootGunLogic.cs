using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.Enums;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;
using WeaponPack.WeaponsLogic.Base;

namespace WeaponPack.WeaponsLogic
{
    public class ShootGunLogic : FireArmBase
    {
        [SerializeField] private float timeBetweenShoots = 0.1f;
        [SerializeField] private float maxGrainTimeBreak = 0.01f;
        
        protected override bool UseWeapon()
        {
            var target = UtilsMethods.FindTargetInBiggestGroup(transform.position);

            if (target == null) return false;
            var position = target.transform.position;
            StartCoroutine(ShootAllMagazines(position));
            return true;
        }

        private IEnumerator ShootAllMagazines(Vector2 position)
        {
            for (var j = 0; j < 2; j++)
            {
                AudioManager.Instance.PlaySound(ESoundType.PistolShoot);
                for (var i = 0; i < ProjectileCount; i++)
                {
                    SpawnProjectile(position)
                        .SetDontDestroyOnHit()
                        .SetReady();
                    
                    var waitTime = Random.Range(0.001f, maxGrainTimeBreak + 0.001f);
                
                    yield return new WaitForSeconds(waitTime);
                }

                yield return new WaitForSeconds(timeBetweenShoots);
            }
        }
    }
}