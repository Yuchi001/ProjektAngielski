using EnemyPack;
using ItemPack.WeaponPack.WeaponsLogic;
using Other;
using Other.Enums;
using PlayerPack;
using ProjectilePack;
using UnityEngine;

namespace ItemPack.WeaponPack.Other
{
    public class FireField : MonoBehaviour
    {
        [SerializeField] private float rangeScaler;
        [SerializeField] private float bodyScale;
            
        private BookOfFireLogic _bookOfFireLogic;
        private TriggerDetector _triggerDetector;
        
        private float _timer = 0;
        
        private Transform particles => transform.GetChild(0);

        public void Setup(BookOfFireLogic bookOfFireLogic)
        {
            _bookOfFireLogic = bookOfFireLogic;
            _triggerDetector = new TriggerDetector(transform, bodyScale);
            _triggerDetector.SetOnTriggerEnter(BurnEnemy);
            _triggerDetector.SetOnTriggerStay(TriggerBurn);
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
            
            _triggerDetector.CheckForTriggers();
        }

        private void TriggerBurn(CanBeDamaged canBeDamaged)
        {
            if (_timer < 1f / _bookOfFireLogic.DamageRate) return;

            _timer = 0;
            BurnEnemy(canBeDamaged);
        }

        private void BurnEnemy(CanBeDamaged canBeDamaged)
        {
            if (canBeDamaged is not EnemyLogic enemy) return;
                
            TriggerDamage(enemy);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, rangeScaler);
        }

        public void TriggerDamage(EnemyLogic enemy)
        {
            enemy.GetDamaged(_bookOfFireLogic.Damage);
            var effectContext = PlayerManager.GetEffectContextManager().GetEffectContext(EEffectType.Burn, _bookOfFireLogic.EffectDuration, enemy);
            enemy.AddEffect(effectContext);
        }
    }
}