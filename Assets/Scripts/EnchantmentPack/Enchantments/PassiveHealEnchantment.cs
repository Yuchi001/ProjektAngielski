using System;
using System.Collections;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class PassiveHealEnchantment : EnchantmentBase, ICooldownEnchantment
    {
        public float MaxCooldown => 1f / parameters[EValueKey.Percentage];

        public float CurrentTime => _timer / MaxCooldown;

        public bool IsActive { get; protected set; }

        private PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;

        private float _timer = 0;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);
        }

        private void Update()
        {
            if (PlayerHealth == null) return;

            IsActive = PlayerHealth.CurrentHealth >= PlayerHealth.MaxHealth;
            if (IsActive) return;

            _timer += Time.deltaTime;
            if (_timer < 1f / parameters[EValueKey.Rate]) return;

            _timer = 0;
            PlayerHealth.Heal((int)(PlayerHealth.MaxHealth * parameters[EValueKey.Value]));
        }
    }
}