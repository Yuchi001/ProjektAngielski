using ItemPack;
using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IDamageContextManager
    {
        public void AddDamageModifier(string key, IDamageModifier modifier);
        public void RemoveDamageModifier(string key);
        public DamageContext GetDamageContext(int damage, ItemLogicBase source);
    }
}