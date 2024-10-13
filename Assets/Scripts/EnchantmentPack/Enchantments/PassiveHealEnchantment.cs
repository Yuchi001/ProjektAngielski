using System;
using System.Collections;
using EnchantmentPack.Interfaces;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class PassiveHealEnchantment : EnchantmentBase, ICooldownEnchantment
    {
        [SerializeField] private float healingFactor = 0.1f;
        [SerializeField] private float healPerSec = 0.1f;

        public float MaxCooldown => 1f / healPerSec;

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
            if (_timer < 1f / healPerSec) return;

            _timer = 0;
            PlayerHealth.Heal((int)(PlayerHealth.MaxHealth * healingFactor));
        }

        public override string GetDescriptionText()
        {
            return $"Restores {(int)(healingFactor * 100)}% max health each {(1 / healPerSec):0.0} seconds.";
        }
    }
}