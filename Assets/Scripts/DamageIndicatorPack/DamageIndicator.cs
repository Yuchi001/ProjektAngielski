using Managers;
using PoolPack;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DamageIndicatorPack
{
    public class DamageIndicator : PoolObject
    {
        [SerializeField] private Gradient damageColorGradient;
        [SerializeField] private Color healColor;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float lifeTime = 0.3f;
        
        private const string DAMAGE_INDICATOR_TIMER_ID = "DAMAGE_INDICATOR_TIMER_ID";

        private IndicatorManager _poolManager;

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _poolManager = (IndicatorManager)poolManager;
            
            transform.SetParent(GameUiManager.Instance.WorldCanvas);
        }

        public void Setup(Vector2 position, int damage, bool isDamage, bool isCrit)
        {
            var randomX = Random.Range(-0.1f, 0.15f);
            position.x += randomX;
            position.y += 0.5f;

            transform.position = position;
            
            var scaledDamage = Mathf.Clamp(damage - 5, 0, 50);
            damageText.color = isDamage ? damageColorGradient.Evaluate(scaledDamage / 50f) : healColor;
            if (isCrit) damageText.color = Color.magenta;
            damageText.text = !isCrit ? damage.ToString() : $"<i>{damage}</i>";
            
            OnGet(null);
            
            SetTimer(DAMAGE_INDICATOR_TIMER_ID);
        }

        public override void InvokeUpdate()
        {
            if (CheckTimer(DAMAGE_INDICATOR_TIMER_ID) < lifeTime) return;
            
            _poolManager.ReleasePoolObject(this);
        }
    }
}