using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class IceWandLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject iceProjectile;
        [SerializeField] private Sprite projectileSprite;

        private Vector2 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        
        protected override void UseWeapon()
        {
            var projectile = Instantiate(iceProjectile, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            var damage = _realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.Damage);
            var speed = _realWeaponStats.FirstOrDefault(s => s.statType == EWeaponStat.Speed);

            if (damage == null || speed == null) return;

            projectileScript.Setup((int)damage.statValue, speed.statValue)
                .SetTarget(UtilsMethods.FindTarget(transform.position))
                .SetSprite(projectileSprite)
                .SetReady();
        }
    }
}