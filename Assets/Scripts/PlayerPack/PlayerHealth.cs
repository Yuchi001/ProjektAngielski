using System.Collections;
using System.Collections.Generic;
using EnchantmentPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Base;
using Managers.Enums;
using Other;
using PlayerPack.SO;
using UI;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
        [SerializeField] private float zoneCheckPerSec = 4;
        [SerializeField] private GameObject healParticles;
        [SerializeField] private GameObject damageIndicator;

        public override int MaxHealth => PickedCharacter.MaxHp;
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
            SpawnNonAloc();
            CanBeDamagedSetup();
        }

        private IEnumerator StartingInvincible()
        {
            Invincible = true;
            yield return new WaitForSeconds(0.5f);
            Invincible = false;
        }

        private void OnEnable()
        {
            _currentHealth = PickedCharacter.MaxHp;
        }

        protected override void OnUpdate()
        {
            ManageZone();
        }

        private void ManageZone()
        {
            _zoneTimer += Time.deltaTime;
            if (_zoneTimer < 1f / zoneCheckPerSec) return;

            _zoneTimer = 0;
            var inBounds = GameManager.Instance.MapGenerator.ContainsEntity(transform.position);
            if (!inBounds) GetDamaged(MaxHealth / 10);
        }

        public override void GetDamaged(int value, Color? color = null)
        {
            if (Dead || Invincible) return;
            
            OnPlayerDamaged?.Invoke();
            
            base.GetDamaged(value, color);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 
                0, PickedCharacter.MaxHp);
            
            AudioManager.Instance.PlaySound(ESoundType.PlayerHurt);
            
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
            AudioManager.Instance.PlaySound(soundType);
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicator, value, false, false);
            
            var particles = Instantiate(healParticles, transform.position, Quaternion.identity);
            Destroy(particles, 1f);
            
            _currentHealth = Mathf.Clamp(_currentHealth + value, 
                0, PickedCharacter.MaxHp);
        }

        public override void OnDie(bool destroyObj = true)
        {
            if (PlayerEnchantments.Ready(EEnchantmentName.Revive))
            {
                OnPlayerRevive?.Invoke();
                Heal(MaxHealth / 2);
                return;
            }
            
            AudioManager.Instance.PlaySound(ESoundType.PlayerDeath);
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

        public override void Setup(SoEntityBase soData)
        {
            // not needed
        }

        public override void SpawnSetup(SpawnerBase spawner)
        {
            // not needed
        }
    }
}