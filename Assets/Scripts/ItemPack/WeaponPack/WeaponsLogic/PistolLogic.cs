using System.Collections;
using AudioPack;
using ItemPack.WeaponPack.WeaponsLogic.Base;
using Managers;
using Managers.Enums;
using TargetSearchPack;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class PistolLogic : FireArmBase
    {
        private NearPlayerStrategy _findStrategy;
        private NearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new NearPlayerStrategy();
            }
        }
        
        protected override bool Use()
        {
            var target = TargetManager.FindTarget(FindStrategy);

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

                SpawnProjectile(position).Ready();
                
                yield return new WaitForSeconds(Cooldown / (2 * ProjectileCount));
            }
        }
    }
}