using System;
using UnityEngine;
using WeaponPack.Other;

namespace Other
{
    public class Walls : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Projectile projectile)) return;
            
            Destroy(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out Projectile projectile)) return;

            if (!projectile.DestroyOnContactWithWall) return;
            
            Destroy(other.gameObject);
        }
    }
}