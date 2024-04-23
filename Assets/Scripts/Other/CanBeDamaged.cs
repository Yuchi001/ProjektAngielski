using System;
using UnityEngine;

namespace Other
{
    public abstract class CanBeDamaged : MonoBehaviour
    {
        [SerializeField] private GameObject bloodParticles;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float flashTime = 0.1f;
        private Material spriteMaterial;

        private void Start()
        {
            spriteMaterial = spriteRenderer.material;
        }

        public virtual void GetDamaged(int value)
        {
            LeanTween.pause(gameObject);
            LeanTween.value(gameObject, 0, 1, flashTime).setOnUpdate(val =>
            {
                spriteMaterial.SetFloat("_FlashAmmount", val);
            }).setOnComplete(() => LeanTween.value(gameObject, 1, 0, flashTime).setOnUpdate(val =>
            {
                spriteMaterial.SetFloat("_FlashAmmount", val);
            }));

            if (bloodParticles == null) return;

            var particles = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
        }

        public virtual void OnDie()
        {
            Destroy(gameObject, flashTime);
        }
    }
}