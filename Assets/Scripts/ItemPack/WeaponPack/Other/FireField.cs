using EnemyPack.CustomEnemyLogic;
using ItemPack.WeaponPack.WeaponsLogic;
using Managers;
using Other.Enums;
using Other.Interfaces;
using PlayerPack;
using UnityEngine;

namespace ItemPack.WeaponPack.Other
{
    public class FireField : MonoBehaviour, IDamageEnemy
    {
        [SerializeField] private float rangeScaler;
            
        private BookOfFireLogic _bookOfFireLogic;
        private Transform particles => transform.GetChild(0);
        
        private float _timer = 0;

        public void Setup(BookOfFireLogic bookOfFireLogic)
        {
            _bookOfFireLogic = bookOfFireLogic;
        }

        private void Update()
        {
            if (!PlayerManager.HasInstance()) return;
            
            var scale = _bookOfFireLogic.Range / rangeScaler;
            var vectorScale = Vector2.one * scale;
            transform.localScale = vectorScale;
            particles.transform.localScale = vectorScale;
            transform.position = PlayerManager.PlayerPos;
            _timer += Time.deltaTime;

            if (_timer < 1f / _bookOfFireLogic.DamageRate) return;

            _timer = 0;
            TriggerBurn();
        }

        private void TriggerBurn()
        {
            var results = new Collider2D[50];
            var playerPos = PlayerManager.PlayerPos;
            var range = _bookOfFireLogic.Range;
            Physics2D.OverlapCircleNonAlloc(playerPos, range, results);
            
            foreach (var hit in results)
            {
                if (hit == null) continue;
                if(!hit.TryGetComponent<EnemyLogic>(out var enemy)) continue;
                
                enemy.GetDamaged(_bookOfFireLogic.Damage);
                enemy.AddEffect(EEffectType.Burn, _bookOfFireLogic.EffectDuration);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, rangeScaler);
        }

        public void TriggerDamage(EnemyLogic enemy)
        {
            enemy.GetDamaged(_bookOfFireLogic.Damage);
            enemy.AddEffect(EEffectType.Burn, _bookOfFireLogic.EffectDuration);
        }
    }
}