using System.Collections;
using System.Collections.Generic;
using EnchantmentPack.Enums;
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
    public class PistolLogic : FireArmBase
    {
        protected override bool UseWeapon()
        {
            var target = UtilsMethods.FindNearestTarget(transform.position);

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

                SpawnProjectile(position).SetReady();
                
                yield return new WaitForSeconds(Cooldown / (2 * ProjectileCount));
            }
        }
    }
}