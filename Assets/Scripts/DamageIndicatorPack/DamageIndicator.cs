using System;
using Managers;
using PoolPack;
using TMPro;
using UIPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DamageIndicatorPack
{
    public class DamageIndicator : SimplePoolObject
    {
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float lifeTime = 0.3f;
        [SerializeField] private Animator animator;

        private float _timer = 0;

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

            _timer = 0;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < lifeTime) return;
            
            _poolManager.ReleasePoolObject(this);
        }
    }
}