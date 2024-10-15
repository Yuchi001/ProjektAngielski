using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpecialEffectPack
{
    [RequireComponent(typeof(Animator))]
    public class ExplosionAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float explosionRangeScaling;

        public ExplosionAnimation Trigger(float range)
        {
            var clip = animator.runtimeAnimatorController.animationClips[0];
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            var time = clip.length / stateInfo.speed;
            Destroy(gameObject, time - 0.05f);
            var scale = range / explosionRangeScaling;
            transform.localScale = Vector2.one * scale;

            return this;
        }

        public ExplosionAnimation SetColor(Color color)
        {
            spriteRenderer.color = color;
            return this;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, explosionRangeScaling);
        }
    }
}