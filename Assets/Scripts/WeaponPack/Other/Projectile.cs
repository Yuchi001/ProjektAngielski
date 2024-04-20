using System;
using EnemyPack;
using UnityEngine;
using Utils;

namespace WeaponPack.Other
{
    [RequireComponent(typeof(Sprite))]
    public class Projectile : MonoBehaviour
    {
        private EnemyLogic _target;
        private Transform Target => _target.transform;
        private int _damage;
        private float _speed;
        
        public Projectile Setup(int damage, float speed)
        {
            UtilsMethods.LookAtMouse(transform);
            _damage = damage;
            _speed = speed;
            return this;
        }

        public Projectile SetSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            return this;
        }

        public Projectile SetTarget(EnemyLogic enemyLogic)
        {
            _target = enemyLogic;
            return this;
        }

        public Projectile SetDirection(Vector3 direction)
        {
            UtilsMethods.LookAtObj(transform, direction);
            return this;
        }

        public Projectile SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, 0);
            return this;
        }

        private void Update()
        {
            var newPos = _target == null ? 
                (Vector2)(transform.position + transform.forward * _speed) :
                Vector2.MoveTowards(transform.position, Target.position, _speed * Time.deltaTime);

            transform.position = newPos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent<EnemyLogic>(out var enemyLogic)) return;
            
            enemyLogic.GetDamaged(_damage);
            Destroy(gameObject);
        }
    }
}