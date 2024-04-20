using System;
using EnemyPack;
using UnityEngine;

namespace WeaponPack.Other
{
    public class Projectile : MonoBehaviour
    {
        private EnemyLogic _enemyLogic;
        private Transform Target => _enemyLogic.transform;
        private int _damage;
        private float _speed;
        
        public void Setup(int damage, float speed, EnemyLogic enemyLogic)
        {
            _enemyLogic = enemyLogic;
            _damage = damage;
            _speed = speed;
        }

        private void Update()
        {
            if (_enemyLogic == null) return;
            
            transform.position = Vector2.MoveTowards(transform.position, Target.position, _speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent<EnemyLogic>(out var enemyLogic)) return;
            
            enemyLogic.GetDamaged(_damage);
            Destroy(gameObject);
        }
    }
}