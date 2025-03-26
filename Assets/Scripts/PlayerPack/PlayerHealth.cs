using System.Collections;
using System.Collections.Generic;
using AudioPack;
using DamageIndicatorPack;
using EnchantmentPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using MapGeneratorPack;
using Other;
using ParticlesPack;
using ParticlesPack.Enums;
using PlayerPack.Enums;
using PlayerPack.SO;
using PoolPack;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
        [SerializeField] private float zoneCheckPerSec = 4;
        [SerializeField] private GameObject healParticles;

        private static PlayerStatsManager PlayerStatsManager => PlayerManager.Instance.PlayerStatsManager;
        public override int MaxHealth => PlayerStatsManager.GetStatAsInt(EPlayerStatType.MaxHealth);
        public override int CurrentHealth => _currentHealth;
        
        private int _currentHealth = 0;

        private float _zoneTimer = 0;

        public bool Invincible { get; set; } = false;

        public delegate void PlayerDamagedDelegate();
        public static event PlayerDamagedDelegate OnPlayerDamaged;

        private static PlayerEnchantments PlayerEnchantments =>
            PlayerManager.Instance.PlayerEnchantments;
        private static SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;

        public delegate void PlayerHealDelegate(int value);
        public static event PlayerHealDelegate OnPlayerHeal;

        public delegate void PlayerReviveDelegate();
        public static event PlayerReviveDelegate OnPlayerRevive;

        private void Awake()
        {
            StartCoroutine(StartingInvincible());
            CanBeDamagedSetup();
        }

        private IEnumerator StartingInvincible()
        {
            Invincible = true;
            yield return new WaitForSeconds(0.5f);
            Invincible = false;
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerStatsManager != null);
            
            _currentHealth = MaxHealth;
        }

        protected void Update()
        {
            if (Dead || !Active) return;
            
            ManageZone();
        }

        private void ManageZone()
        {
            _zoneTimer += Time.deltaTime;
            if (_zoneTimer < 1f / zoneCheckPerSec) return;

            _zoneTimer = 0;
            var inBounds = MapGenerator.ContainsEntity(transform.position);
            if (!inBounds) GetDamaged(MaxHealth / 10);
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            if (Dead || Invincible) return;
            
            base.GetDamaged(value, color);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 
                0, MaxHealth);
            
            AudioManager.PlaySound(ESoundType.PlayerHurt);
            
            OnPlayerDamaged?.Invoke();
            if(_currentHealth <= 0) OnDie(false);
        }

        public void Heal(int value, ESoundType soundType = ESoundType.Heal)
        {
            // dont change sequence of this array it needs to be sorted like this
            var betterHealLevelArr = new List<EEnchantmentName>
            {
                EEnchantmentName.BetterHeal2,
                EEnchantmentName.BetterHeal1,
                EEnchantmentName.BetterHeal,
            };
            foreach (var enchantment in betterHealLevelArr)
            {
                if (!PlayerEnchantments.Has(enchantment)) continue;
                value = Mathf.CeilToInt(value * (1 + PlayerEnchantments.GetParamValue(enchantment, EValueKey.Percentage)));
            }
            
            OnPlayerHeal?.Invoke(value);
            AudioManager.PlaySound(soundType);
            
            IndicatorManager.SpawnIndicator(PlayerManager.Instance.PlayerPos, value, false, false);
            ParticleManager.SpawnParticles(EParticlesType.Heal, PlayerManager.Instance.PlayerPos);
            
            _currentHealth = Mathf.Clamp(_currentHealth + value, 0, MaxHealth);
        }

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            if (PlayerEnchantments.Ready(EEnchantmentName.Revive))
            {
                OnPlayerRevive?.Invoke();
                Heal(MaxHealth / 2);
                return;
            }
            
            AudioManager.PlaySound(ESoundType.PlayerDeath);
            PlayerManager.Instance.ManagePlayerDeath();
            
            base.OnDie(destroyObj);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Projectile projectile)) return;
            
            projectile.ManageHit(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out Projectile projectile)) return;
            
            projectile.ManageHit(gameObject);
        }
    }
}