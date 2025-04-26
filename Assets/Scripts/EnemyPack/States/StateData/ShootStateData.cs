using System.Collections.Generic;
using PlayerPack;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using UnityEngine;

namespace EnemyPack.States.StateData
{
    [System.Serializable]
    public class ShootStateData : AttackStateData
    {
        [SerializeField] private List<Sprite> bulletSprites;
        [SerializeField] private float changeBulletSpriteRate;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletScale;

        public virtual void Shoot(EnemyLogic state)
        {
            var spawnPos = state.transform.position;
            var movementStrategy = new DirectionMovementStrategy(spawnPos, PlayerManager.PlayerPos, bulletSpeed);
            var projectile = ProjectileManager.SpawnProjectile(movementStrategy, damage, spawnPos, ProjectileManager.PLAYER_TAG);
            projectile
                .SetRange(range)
                .SetSprite(bulletSprites, changeBulletSpriteRate)
                .SetScale(bulletScale)
                .Ready();
        }
    }
}