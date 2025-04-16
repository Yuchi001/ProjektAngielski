using System.Collections.Generic;
using EnchantmentPack.Enums;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfFireLogic : ItemLogicBase
    {
        private FireField _spawnedFireField = null;

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
            if (_spawnedFireField != null) return false;

            var prefab = GameManager.GetPrefab<FireField>(PrefabNames.FireField);
            _spawnedFireField = Instantiate(prefab);
            _spawnedFireField.Setup(this);

            return false;
        }

        public override void Remove()
        {
            Destroy(_spawnedFireField.gameObject);
            base.Remove();
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