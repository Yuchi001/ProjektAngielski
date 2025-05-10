using System;
using System.Collections;
using System.Threading.Tasks;
using Managers;
using Managers.Other;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace DamageIndicatorPack
{
    public class IndicatorManager : PoolManager, IMainSingleton
    {
        [SerializeField] private Gradient damageColorGradient;
        [SerializeField] private Color healColor;
        
        #region Singleton

        private static IndicatorManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        private ObjectPool<DamageIndicator> _pool;

        public void Init()
        {
            var prefab = GameManager.GetPrefab<DamageIndicator>(PrefabNames.DamageIndicatorHolder);
            _pool = PoolHelper.CreatePool(this, prefab, false);
        }
        
        protected override T GetPoolObject<T>()
        {
            return _pool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _pool.Release((DamageIndicator)poolObject);
        }

        public static void SpawnIndicator(Vector2 position, int value, bool isCrit, bool isDamage = true)
        {
            var scaledDamage = Mathf.Clamp(value - 5, 0, 50);
            var color = isDamage ? Instance.damageColorGradient.Evaluate(scaledDamage / 50f) : Instance.healColor;
            if (isCrit) color = Color.magenta;
            var text = !isCrit ? value.ToString() : $"<i>{value}</i>";
            Instance.GetPoolObject<DamageIndicator>().Setup(position, text, color);
        }

        public static void SpawnIndicator(Vector2 position, string text, Color color)
        {
            Instance.GetPoolObject<DamageIndicator>().Setup(position, text, color);
        }
    }
}