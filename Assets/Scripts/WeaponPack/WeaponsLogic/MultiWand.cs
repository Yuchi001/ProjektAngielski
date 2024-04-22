using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class MultiWand : WeaponLogicBase
    {
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject projectilePrefab;
        protected override void UseWeapon()
        {
            StartCoroutine(SpawnParticles());
        }

        private IEnumerator SpawnParticles()
        {
            for (var i = 0; i < ProjectileCount; i++)
            {
                var position = PlayerTransform.position;
                var up = PlayerTransform.up;
                var right = PlayerTransform.right;
            
                const int rotCount = 4;
            
                var rotations = new List<Vector2>(rotCount)
                {
                    up + position,
                    -up + position,
                    right + position,
                    -right + position,
                };
                
                for (var j = 0; j < rotCount; j++)
                {
                    var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                    var projectileScript = projectile.GetComponent<Projectile>();

                    Debug.Log(rotations[j]);
                    
                    projectileScript.Setup(Damage, Speed)
                        .SetSprite(projectileSprite)
                        .SetDirection(rotations[j])
                        .SetReady();
                }
                
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}