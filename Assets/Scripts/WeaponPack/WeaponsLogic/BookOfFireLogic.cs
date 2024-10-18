using System;
using EnchantmentPack.Enums;
using Managers;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class BookOfFireLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject fireFieldPrefab;
        
        private bool _spawned = false;

        public float Range => GetStatValue(EWeaponStat.ProjectileRange) ?? 0;
        public float DamageRate => (GetStatValue(EWeaponStat.DamageRate) ?? 0) * CustomRateModifier();
        public float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;

        private void LateUpdate()
        {
            //Debug.Log(DamageRate);
        }

        protected override bool UseWeapon()
        {
            if (_spawned) return false;

            var fieldObj = Instantiate(fireFieldPrefab);
            fieldObj.GetComponent<FireField>().Setup(this);

            _spawned = true;

            return false;
        }
        
        private float CustomRateModifier()
        {
            var stacks = PlayerEnchantmentManager.GetStacks(EEnchantmentName.BetterBooks);
            if (stacks <= 0) return base.CustomCooldownModifier();
            return 1 + GameManager.Instance.EnchantmentValueDictionary[EEnchantmentName.BetterBooks][EValueKey.Percentage] * stacks;
        }
    }
}