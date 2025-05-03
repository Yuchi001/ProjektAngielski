using Other;
using Other.Enums;
using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IEffectContextManager
    {
        public void AddEffectModifier(string key, IEffectModifier modifier);
        public void RemoveEffectModifier(string key);
        public EffectContext GetEffectContext(EEffectType effectType, float duration, CanBeDamaged canBeDamaged);
    }
}