using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class MultiWandLogic : WeaponLogicBase
    {
        [SerializeField] private Color projectileLightColor;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private GameObject projectilePrefab;
        protected override bool UseWeapon()
        {
            StartCoroutine(SpawnParticles());

            return true;
        }

        private IEnumerator SpawnParticles()
        {
            var alpha = 90f / ProjectileCount;

            var currentAngle = alpha;
            
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
                    
                    projectileScript.Setup(Damage, Speed)
                        .SetSprite(projectileSprite)
                        .SetDirection(rotations[j], currentAngle)
                        .SetLightColor(projectileLightColor)
                        .SetReady();
                }
                
                currentAngle += alpha;
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}