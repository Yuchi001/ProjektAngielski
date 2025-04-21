using UnityEngine;

namespace ProjectilePack
{
    public interface IProjectileMovementStrategy
    {
        public void MoveProjectile(Transform bulletTransform);
    }
}