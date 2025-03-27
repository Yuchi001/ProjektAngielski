using System;
using System.Collections;
using System.Collections.Generic;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class ReviveEnchantment : EnchantmentBase, ICooldownEnchantment
    {
        public float MaxCooldown => parameters.GetValueOrDefault(EValueKey.Time, 0);

        public float CurrentTime => _timer / MaxCooldown;
        public bool IsActive { get; private set; } = false;

        private float _timer;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);
            
            _timer = MaxCooldown;
            PlayerHealth.OnPlayerRevive += OnRevive;
        }

        private void Update()
        {
            if (IsActive) return;
            
            if (_timer < MaxCooldown) _timer += Time.deltaTime;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerRevive -= OnRevive;
        }

        private void OnRevive()
        {
            _timer = 0;
        }
    }
}