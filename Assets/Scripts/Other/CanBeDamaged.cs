using System.Collections;
using EffectPack;
using Other.Enums;
using ParticlesPack;
using ParticlesPack.Enums;
using PoolPack;
using UnityEngine;

namespace Other
{
    
    //TODO: Refactor
    public abstract class CanBeDamaged : PoolObject
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float _flashTime = 0.1f;
        private Material _spriteMaterial;
        
        private Coroutine _currentCoroutine = null;
        protected EffectsManager _effectsManager => GetComponentInChildren<EffectsManager>();

        public bool Dead { get; private set; }
        
        public abstract int CurrentHealth { get; }
        public abstract int MaxHealth { get; }

        protected bool Stuned => _effectsManager.Stuned;
        protected bool Slowed => _effectsManager.Slowed;

        public SpriteRenderer SpriteRenderer => spriteRenderer;


        protected void CanBeDamagedSetup()
        {
            Dead = false;
            _currentCoroutine = null;
            _spriteMaterial = spriteRenderer.material;
            _spriteMaterial.SetColor("_FlashColor", Color.white);
            _spriteMaterial.SetFloat("_FlashAmmount", 0);
            if (_effectsManager) _effectsManager.Setup(this);
        }

        public virtual void AddEffect(EEffectType effectType, float duration)
        {
            _effectsManager.AddEffect(effectType, duration);
        }

        public bool HasEffect(EEffectType effectType)
        {
            return _effectsManager.HasEffect(effectType);
        }
        
        public virtual void GetDamaged(int value, Color? flashColor = null)
        {
            flashColor ??= Color.white;
            _currentCoroutine ??= StartCoroutine(DamageAnim(flashColor.Value));

            SpawnBlood();
        }

        public void SpawnBlood()
        {
            ParticleManager.SpawnParticles(EParticlesType.Blood, transform.position);
        }

        public virtual void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            Dead = true;
            _spriteMaterial.SetColor("_FlashColor", Color.red);
            _spriteMaterial.SetFloat("_FlashAmmount", 1);
            StartCoroutine(Die(destroyObj, poolManager));
        }

        private IEnumerator Die(bool destroyObj, PoolManager poolManager)
        {
            for (float time = 0; time < _flashTime; time+=Time.deltaTime)
            {
                transform.localScale = new Vector3(1f - time / _flashTime, 1, 1);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (!destroyObj) yield break;
            
            if (poolManager == null) Destroy(gameObject);
            else poolManager.ReleasePoolObject(this);
        }
        
        private IEnumerator DamageAnim(Color flashColor)
        {
            _spriteMaterial.SetColor("_FlashColor", flashColor);
            _spriteMaterial.SetFloat("_FlashAmmount", 1);
            yield return new WaitForSeconds(_flashTime);
            _spriteMaterial.SetFloat("_FlashAmmount", 0);

            _currentCoroutine = null;
        }
    }
}