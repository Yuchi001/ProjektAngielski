using System.Collections;
using AudioPack;
using ItemPack.WeaponPack.WeaponsLogic.Base;
using Managers.Enums;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class ShootGunLogic : FireArmBase
    {
        [SerializeField] private float timeBetweenShoots = 0.1f;
        [SerializeField] private float maxGrainTimeBreak = 0.01f;

        private BiggestGroupNearPlayerStrategy _findStrategy;
        private BiggestGroupNearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new BiggestGroupNearPlayerStrategy(new NearPlayerStrategy());
            }
        }
        
        protected override bool Use()
        {
            var target = FindTarget(FindStrategy, 20f);

            if (target == null) return false;
            var position = target.transform.position;
            StartCoroutine(ShootAllMagazines(position));
            return true;
        }

        private IEnumerator ShootAllMagazines(Vector2 position)
        {
            for (var j = 0; j < 2; j++)
            {
                AudioManager.PlaySound(ESoundType.PistolShoot);
                for (var i = 0; i < ProjectileCount; i++)
                {
                    SpawnProjectile(position)
                        .SetDestroyOnCollision(false)
                        .Ready();
                    
                    var waitTime = Random.Range(0.001f, maxGrainTimeBreak + 0.001f);
                
                    yield return new WaitForSeconds(waitTime);
                }

                yield return new WaitForSeconds(timeBetweenShoots);
            }
        }
    }
}