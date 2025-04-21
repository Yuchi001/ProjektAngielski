using System.Collections.Generic;
using ItemPack;
using ItemPack.Enums;
using Other;

namespace PlayerPack.Decorators
{
    public class DamageContext
    {
        public int Damage { get; private set; }
        public List<EItemTag> Tags { get; private set; }
        public CanBeDamaged HitObj { get; }

        public DamageContext(int damage, List<EItemTag> tags, CanBeDamaged hitObj)
        {
            Damage = damage;
            Tags = tags;
            HitObj = hitObj;
        }

        public void SetTags(List<EItemTag> tags) => Tags = tags;
        public void SetDamage(int damage) => Damage = damage;
    }
}