using System.Collections.Generic;
using EnchantmentPack.Enums;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfFireLogic : ItemLogicBase
    {
        [SerializeField] private GameObject fireFieldPrefab;
        
        private bool _spawned = false;

        public float Range => GetStatValue(EItemSelfStatType.ProjectileRange);
        public float DamageRate => GetStatValue(EItemSelfStatType.DamageRate) * CustomRateModifier();
        public float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileRange,
            EItemSelfStatType.DamageRate,
            EItemSelfStatType.EffectDuration,
            EItemSelfStatType.Damage
        };

        protected override bool Use()
        {
            if (_spawned) return false;

            var fieldObj = Instantiate(fireFieldPrefab);
            fieldObj.GetComponent<FireField>().Setup(this);

            _spawned = true;

            return false;
        }
        
        private float CustomRateModifier()
        {
            var stacks = PlayerEnchantments.GetStacks(EEnchantmentName.BetterBooks);
            if (stacks <= 0) return base.CustomCooldownModifier();
            var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.BetterBooks, EValueKey.Percentage);
            return 1 + percentage * stacks;
        }
    }
}