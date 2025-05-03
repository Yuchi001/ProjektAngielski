using System.Collections.Generic;
using ItemPack;
using ItemPack.Enums;
using Other;
using UnityEngine;

namespace PlayerPack.Decorators
{
    public class DamageContext
    {
        public int Damage { get; private set; }
        public List<EItemTag> Tags { get; private set; }
        public CanBeDamaged HitObj { get; }
        public MonoBehaviour Source { get; }

        public DamageContext(int damage, MonoBehaviour source, CanBeDamaged hitObj, List<EItemTag> tags)
        {
            Damage = damage;
            Tags = tags;
            HitObj = hitObj;
            Source = source;
        }

        public void SetTags(List<EItemTag> tags) => Tags = tags;
        public void SetDamage(int damage) => Damage = damage;
    }
}