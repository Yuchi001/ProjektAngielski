using System;
using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Managers.Enums;
using Other.Enums;
using PlayerPack;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using WeaponPack.SO;
using WeaponPack.WeaponsLogic;

namespace WeaponPack.Other
{
    public class FireField : MonoBehaviour
    {
        [SerializeField] private float rangeScaler;
            
        private BookOfFireLogic _bookOfFireLogic;
        private Transform particles => transform.GetChild(0);

        private Vector2 Position
        {
            get
            {
                var pp = PlayerManager.Instance.transform.position;
                pp.y -= 0.2f;
                return pp;
            }
        }

        private float _timer = 0;

        public void Setup(BookOfFireLogic bookOfFireLogic)
        {
            _bookOfFireLogic = bookOfFireLogic;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;
            
            var scale = _bookOfFireLogic.Range / rangeScaler;
            var vectorScale = Vector2.one * scale;
            transform.localScale = vectorScale;
            particles.transform.localScale = vectorScale;
            transform.position = Position;
            _timer += Time.deltaTime;

            if (_timer < 1f / _bookOfFireLogic.DamageRate) return;

            _timer = 0;
            TriggerBurn();
        }

        private void TriggerBurn()
        {
            var results = new Collider2D[50];
            var playerPos = PlayerManager.Instance.transform.position;
            var range = _bookOfFireLogic.Range;
            Physics2D.OverlapCircleNonAlloc(playerPos, range, results);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(_bookOfFireLogic.Damage);
                enemy.AddEffect(new EffectInfo
                {
                    effectType = EEffectType.Burn,
                    time = _bookOfFireLogic.EffectDuration,
                });
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, rangeScaler);
        }
    }
}