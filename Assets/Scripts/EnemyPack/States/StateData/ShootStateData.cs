using System.Collections.Generic;
using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class ShootStateData : MeleeAttackStateData
    {
        [SerializeField] private List<Sprite> bulletSprites;
        [SerializeField] private float bulletAnimationRate;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletScale;

        public List<Sprite> BulletSprites => bulletSprites;
        public float BulletAnimationRate => bulletAnimationRate;
        public float BulletSpeed => bulletSpeed;
        public float BulletScale => bulletScale;
    }
}