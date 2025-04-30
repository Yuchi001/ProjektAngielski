using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class CircleShootStateData : ShootStateData
    {
        [SerializeField] private int bulletCount;

        public int BulletCount => bulletCount;
    }
}