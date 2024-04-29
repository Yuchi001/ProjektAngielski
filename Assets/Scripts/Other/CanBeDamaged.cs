using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Other
{
    public abstract class CanBeDamaged : MonoBehaviour
    {
        [SerializeField] private GameObject bloodParticles;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float _flashTime = 0.1f;
        private Material _spriteMaterial;

        public bool Dead { get; private set; }

        private void Start()
        {
            Dead = false;
            _spriteMaterial = spriteRenderer.material;
        }

        protected void Update()
        {
            if(Dead) return;
            
            OnUpdate();
        }

        protected abstract void OnUpdate();

        public virtual void GetDamaged(int value)
        {
            LeanTween.pause(gameObject);
            LeanTween.value(gameObject, 0, 1, _flashTime).setOnUpdate(val =>
            {
                _spriteMaterial.SetFloat("_FlashAmmount", val);
            }).setOnComplete(() => LeanTween.value(gameObject, 1, 0, _flashTime).setOnUpdate(val =>
            {
                _spriteMaterial.SetFloat("_FlashAmmount", val);
            }));

            if (bloodParticles == null) return;

            var particles = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        public virtual void OnDie()
        {
            Dead = true;
            LeanTween.pause(gameObject);
            _spriteMaterial.SetColor("_FlashColor", Color.red);
            LeanTween.value(gameObject, 0, 1, _flashTime).setOnUpdate(val =>
            {
                _spriteMaterial.SetFloat("_FlashAmmount", val);
            }).setOnComplete(() => LeanTween.value(gameObject, 1, 0, _flashTime).setOnUpdate(val =>
            {
                _spriteMaterial.SetFloat("_FlashAmmount", val);
            }));
            LeanTween.scaleX(gameObject, 0, 0.2f).setOnComplete(() => Destroy(gameObject));
        }
    }
}