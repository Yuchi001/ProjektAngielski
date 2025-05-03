using Other;
using Other.Enums;
using UnityEngine;

namespace EffectPack.SO
{
    public abstract class SoEffectBase : ScriptableObject
    {
        [Header("Settings")] 
        [SerializeField] private bool isContinues;
        [SerializeField, Min(0.1f)] private int resolvePerSecond;
        [SerializeField] private bool canStack;
        
        [Header("Visual")]
        [SerializeField] private string effectName;
        [SerializeField] private Sprite effectSprite;
        [SerializeField] private ParticleSystem effectParticles;
        [SerializeField] private Color effectColor;
        [SerializeField] private EEffectType effectType;

        public EEffectType EffectType => effectType;
        public string EffectName => effectName;
        public Sprite EffectSprite => effectSprite;
        public ParticleSystem EffectParticles => effectParticles;
        public Color EffectColor => effectColor;

        public bool IsCountinues => isContinues;
        public float ResolveRate => 1f / resolvePerSecond;
        public bool CanStack => canStack;

        /// <summary>
        /// Called also when removing effect
        /// </summary>
        public abstract void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged, int additionalDamage);
        public virtual void OnAdd(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged){}
    }
}