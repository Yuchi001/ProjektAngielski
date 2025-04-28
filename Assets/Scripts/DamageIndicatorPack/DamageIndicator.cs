using Managers;
using PoolPack;
using TMPro;
using UIPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DamageIndicatorPack
{
    public class DamageIndicator : PoolObject
    {
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float lifeTime = 0.3f;
        [SerializeField] private Animator animator;
        
        private const string DAMAGE_INDICATOR_TIMER_ID = "DAMAGE_INDICATOR_TIMER_ID";

        private IndicatorManager _poolManager;

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _poolManager = (IndicatorManager)poolManager;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            
            transform.SetParent(UIManager.WorldCanvas);
        }

        public void Setup(Vector2 position, string message, Color color)
        {
            var randomX = Random.Range(-0.1f, 0.15f);
            position.x += randomX;
            position.y += 0.5f;

            transform.position = position;

            damageText.text = message;
            damageText.color = color;
            
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