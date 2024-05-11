using EnemyPack;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class BookOfPoisonLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GameObject fieldParticles;
        [SerializeField] private Sprite fieldSprite;

        private float Duration => GetStatValue(EWeaponStat.Duration) ?? 0;
        private float Scale => GetStatValue(EWeaponStat.ProjectileScale) ?? 0;
        private float DamageRate => GetStatValue(EWeaponStat.DamageRate) ?? 1;

        private const string DamageRateName = "DamageRate";
        
        protected override void UseWeapon()
        {
            var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.Setup(Damage, 0)
                .SetScale(Scale / 2f)
                .SetSprite(fieldSprite, Scale * 2)
                .SetLightColor(Color.clear)
                .SetLifeTime(Duration)
                .SetDontDestroyOnHit()
                .SetNewCustomValue(DamageRateName)
                .SetUpdate(ParticleUpdate)
                .SetOnCollisionStay(CollisionStay)
                .SetFlightParticles(fieldParticles, Scale * 2, true)
                .SetReady();
        }

        private void ParticleUpdate(Projectile projectile)
        {
            var currentRate = projectile.GetCustomValue(DamageRateName);
            projectile.SetCustomValue(DamageRateName, currentRate + Time.deltaTime);
        }

        private void CollisionStay(GameObject enemy, Projectile projectile)
        {
            if (projectile.GetCustomValue(DamageRateName) < 1f / DamageRate) return;
            
            var enemyScript = enemy.GetComponent<EnemyLogic>();
            enemyScript.GetDamaged(Damage);
            projectile.SetCustomValue(DamageRateName, 0);
        }
    }
}