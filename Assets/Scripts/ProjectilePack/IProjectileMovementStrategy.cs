namespace ProjectilePack
{
    public interface IProjectileMovementStrategy
    {
        public void MoveProjectile(Projectile projectile, float deltaTime);

        public static IProjectileMovementStrategy IGNORE => new IgnoreMovementStrategy();
        
        public class IgnoreMovementStrategy : IProjectileMovementStrategy
        {
            public void MoveProjectile(Projectile projectile, float deltaTime) { } // IGNORE
        }
    }
}