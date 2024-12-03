using EnchantmentPack.Enums;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfFireLogic : ItemLogicBase
    {
        [SerializeField] private GameObject fireFieldPrefab;
        
        private bool _spawned = false;

        public float Range => GetStatValue(EWeaponStat.ProjectileRange) ?? 0;
        public float DamageRate => (GetStatValue(EWeaponStat.DamageRate) ?? 0) * CustomRateModifier();
        public float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;
        

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