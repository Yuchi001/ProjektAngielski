using System.Collections.Generic;
using System.Linq;
using PoolPack;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace SpecialEffectPack
{
    [RequireComponent(typeof(Animator))]
    public class ExplosionAnimation : PoolObject
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private List<ExplosionData> explosionDataList;

        private SpecialEffectManager _pool;
        private float _timer = 0;

        private Dictionary<ESpecialEffectType, ExplosionData> _explosionDataDict = new();

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _pool = poolManager as SpecialEffectManager;
            _explosionDataDict = explosionDataList.ToDictionary(e => e.SpecialEffectType, e => e);
        }

        public void Setup(ESpecialEffectType type, float range, Color color = default)
        {
            if (!_explosionDataDict.TryGetValue(type, out var data)) return;
            
            animator.Play(data.AnimName);
            
            var scale = range / data.ExplosionRangeScaling;
            transform.localScale = Vector2.one * scale;

            _timer = 0;
            
            if (color != default) spriteRenderer.color = color;
            
            OnGet(null);
        }

        private float GetLifeTime()
        {
            var clip = animator.runtimeAnimatorController.animationClips[0];
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            return clip.length / stateInfo.speed - 0.1f;
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < GetLifeTime()) return;
            
            _pool.ReleasePoolObject(this);
        }

        [System.Serializable]
        private class ExplosionData
        {
            [SerializeField] private string animName;
            [SerializeField] private float explosionRangeScaling;
            [SerializeField] private ESpecialEffectType specialEffectType;
            
            public string AnimName => animName;
            public float ExplosionRangeScaling => explosionRangeScaling;
            public ESpecialEffectType SpecialEffectType => specialEffectType;
        }
    }
}