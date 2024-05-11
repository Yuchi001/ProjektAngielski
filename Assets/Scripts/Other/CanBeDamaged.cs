using System;
using System.Collections;
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

        private Coroutine _currentCoroutine = null;

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
            _currentCoroutine ??= StartCoroutine(DamageAnim());

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
        
        private IEnumerator DamageAnim()
        {
            _spriteMaterial.SetFloat("_FlashAmmount", 1);
            yield return new WaitForSeconds(_flashTime);
            _spriteMaterial.SetFloat("_FlashAmmount", 0);
        }
    }
}