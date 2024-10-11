using System;
using System.Collections;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class PassiveHealEnchantment : EnchantmentBase
    {
        [SerializeField] private float healingFactor = 0.1f;
        [SerializeField] private float healPerSec = 0.1f;
        private PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;

        private float _timer = 0;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);
        }

        private void Update()
        {
            if (PlayerHealth == null) return;

            if (PlayerHealth.CurrentHealth >= PlayerHealth.MaxHealth) return;

            _timer += Time.deltaTime;
            if (_timer < 1f / healPerSec) return;

            _timer = 0;
            PlayerHealth.Heal((int)(PlayerHealth.MaxHealth * healingFactor));
        }
    }
}