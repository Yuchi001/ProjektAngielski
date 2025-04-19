using System.Collections.Generic;
using ItemPack;
using ItemPack.Enums;

namespace PlayerPack.Decorators
{
    public class DamageContext
    {
        public int Damage { get; private set; }
        public List<EItemTag> Tags { get; private set; }
        public ItemLogicBase Source { get; }

        public DamageContext(int damage, List<EItemTag> tags, ItemLogicBase source)
        {
            Damage = damage;
            Tags = tags;
            Source = source;
        }

        public void SetTags(List<EItemTag> tags) => Tags = tags;
        public void SetDamage(int damage) => Damage = damage;
    }
}