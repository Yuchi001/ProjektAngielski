using Other;
using Other.Enums;
using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IHealContextManager
    {
        public void AddHealModifier(string key, IHealModifier modifier);
        public void RemoveHealModifier(string key);
        public HealContext GetHealContext(int value, CanBeDamaged canBeDamaged);
    }
}