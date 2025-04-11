using System;
using System.Collections;
using System.Collections.Generic;
using AudioPack;
using DamageIndicatorPack;
using EnchantmentPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Enums;
using Other;
using ParticlesPack;
using ParticlesPack.Enums;
using PlayerPack.Enums;
using PlayerPack.SO;
using PoolPack;
using UnityEngine;
using WorldGenerationPack;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
        [SerializeField] private float zoneCheckPerSec = 4;

        private static PlayerStatsManager PlayerStatsManager => PlayerManager.PlayerStatsManager;
        public override int MaxHealth => PlayerStatsManager.GetStatAsInt(EPlayerStatType.MaxHealth);
        public override int CurrentHealth => _currentHealth;
        
        private int _currentHealth = 0;

        private float _zoneTimer = 0;

        public bool Invincible { get; set; } = false;

        public delegate void PlayerDamagedDelegate(int damage, int current);
        public static event PlayerDamagedDelegate OnPlayerDamaged;

        private static PlayerEnchantments PlayerEnchantments => PlayerManager.PlayerEnchantments;
        private static SoCharacter PickedCharacter => PlayerManager.PickedCharacter;

        public delegate void PlayerHealDelegate(int healed, int current);
        public static event PlayerHealDelegate OnPlayerHeal;

        public delegate void PlayerReviveDelegate();
        public static event PlayerReviveDelegate OnPlayerRevive;

        private void Awake()
        {
            OnGet(null);

            PlayerManager.OnChangeState += OnChangeState;
        }
        
        private void OnDisable()
        {
            PlayerManager.OnChangeState -= OnChangeState;
        }

        private void OnChangeState(PlayerManager.State state)
        {
            StartCoroutine(StartingInvincible());
        }
        
        private IEnumerator StartingInvincible()
        {
            Invincible = true;
            yield return new WaitForSeconds(0.5f);
            Invincible = false;
        }

        private IEnumerator Start()
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
            if (PlayerManager.CurrentState != PlayerManager.State.ON_MISSION) return;
            
            _zoneTimer += Time.deltaTime;
            if (_zoneTimer < 1f / zoneCheckPerSec) return;

            _zoneTimer = 0;
            var inBounds = ZoneGeneratorManager.ContainsEntity(transform.position);
            if (!inBounds) GetDamaged(MaxHealth / 10);
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            if (Dead || Invincible) return;
            
            base.GetDamaged(value, color);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 
                0, MaxHealth);
            
            AudioManager.PlaySound(ESoundType.PlayerHurt);
            
            OnPlayerDamaged?.Invoke(value, _currentHealth);
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
            
            AudioManager.PlaySound(soundType);
            
            IndicatorManager.SpawnIndicator(PlayerManager.PlayerPos, value, false, false);
            ParticleManager.SpawnParticles(EParticlesType.Heal, PlayerManager.PlayerPos);
            
            _currentHealth = Mathf.Clamp(_currentHealth + value, 0, MaxHealth);
            
            OnPlayerHeal?.Invoke(value, _currentHealth);
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
            PlayerManager.ManagePlayerDeath();
            
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