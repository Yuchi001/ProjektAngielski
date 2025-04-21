using ProjectilePack.MovementStrategies;
using UnityEngine;

namespace ProjectilePack
{
    public interface IProjectileMovementStrategy
    {
        public void MoveProjectile(Projectile projectile);

        public static IProjectileMovementStrategy IGNORE => new IgnoreMovementStrategy();
        
        public class IgnoreMovementStrategy : IProjectileMovementStrategy
        {
            public void MoveProjectile(Projectile projectile) { } // IGNORE
        }
    }
}