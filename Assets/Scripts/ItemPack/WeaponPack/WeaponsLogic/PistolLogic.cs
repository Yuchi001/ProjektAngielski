using System.Collections;
using AudioPack;
using ItemPack.WeaponPack.WeaponsLogic.Base;
using Managers;
using Managers.Enums;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class PistolLogic : FireArmBase
    {
        protected override bool Use()
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
                AudioManager.PlaySound(ESoundType.PistolShoot);

                SpawnProjectile(position).SetReady();
                
                yield return new WaitForSeconds(Cooldown / (2 * ProjectileCount));
            }
        }
    }
}