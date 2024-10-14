using System.Collections;
using Managers;
using Other.Enums;
using UnityEngine;

namespace Other
{
    public abstract class CanBeDamaged : MonoBehaviour
    {
        [SerializeField] private GameObject effectsManagerPrefab;
        [SerializeField] private GameObject bloodParticles;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float _flashTime = 0.1f;
        private Material _spriteMaterial;
        
        private Coroutine _currentCoroutine = null;
        protected EffectsManager _effectsManager;

        public bool Dead { get; private set; }
        
        public abstract int CurrentHealth { get; }
        public abstract int MaxHealth { get; }

        protected bool Stuned => _effectsManager.Stuned;
        protected bool Slowed => _effectsManager.Slowed;

        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        private void Start()
        {
            Dead = false;
            _spriteMaterial = spriteRenderer.material;
            _effectsManager = Instantiate(effectsManagerPrefab, transform).GetComponent<EffectsManager>();
            _effectsManager.Setup(this);
        }

        protected void Update()
        {
            if (Dead || Stuned) return;
            
            OnUpdate();
        }

        public virtual void AddEffect(EffectInfo effectInfo)
        {
            _effectsManager.AddEffect(effectInfo.effectType, effectInfo.time);
        }

        public bool HasEffect(EEffectType effectType)
        {
            return _effectsManager.HasEffect(effectType);
        }

        protected abstract void OnUpdate();

        public virtual void GetDamaged(int value, Color? flashColor = null)
        {
            flashColor ??= Color.white;
            _currentCoroutine ??= StartCoroutine(DamageAnim(flashColor.Value));

            if (bloodParticles == null) return;

            var particles = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        public virtual void OnDie(bool destroyObj = true)
        {
            Dead = true;
            _spriteMaterial.SetColor("_FlashColor", Color.red);
            _spriteMaterial.SetFloat("_FlashAmmount", 1);
            StartCoroutine(Die(destroyObj));
        }

        private IEnumerator Die(bool destroyObj)
        {
            for (float time = 0; time < _flashTime; time+=Time.deltaTime)
            {
                transform.localScale = new Vector3(1f - time / _flashTime, 1, 1);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            if(destroyObj) Destroy(gameObject);
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