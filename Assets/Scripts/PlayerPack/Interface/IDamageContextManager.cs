using System.Collections.Generic;
using ItemPack;
using ItemPack.Enums;
using Other;
using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IDamageContextManager
    {
        public void AddDamageModifier(string key, IDamageModifier modifier);
        public void RemoveDamageModifier(string key);
        public DamageContext GetDamageContext(int damage, CanBeDamaged hitObject, List<EItemTag> itemTags = null);
    }
}