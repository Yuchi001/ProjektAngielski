using Other;
using Other.Enums;

namespace PlayerPack.Decorators
{
    public class EffectContext
    {
        public EEffectType EffectType { get; private set; }
        public float Duration { get; private set; }
        public CanBeDamaged CanBeDamaged { get; }
        public bool ForceStackingEffect { get; private set; }

        public EffectContext(EEffectType effectType, float duration, CanBeDamaged canBeDamaged)
        {
            EffectType = effectType;
            Duration = duration;
            CanBeDamaged = canBeDamaged;
        }

        public void SetForceStackingEffect() => ForceStackingEffect = true;
        public void ModifyDuration(float duration) => Duration = duration;
        public void ModifyEffectType(EEffectType effectType) => EffectType = effectType;
    }
}