using System.Collections;
using AudioPack;
using DamageIndicatorPack;
using Managers.Enums;
using Other;
using ParticlesPack;
using ParticlesPack.Enums;
using PlayerPack.Enums;
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

        public delegate void PlayerHealDelegate(int healValue, int current);
        public static event PlayerHealDelegate OnPlayerHeal;

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
            
            _currentHealth = Mathf.Clamp(_currentHealth - value, 0, MaxHealth);
            OnPlayerDamaged?.Invoke(value, _currentHealth);
            base.GetDamaged(value, color);
            
            AudioManager.PlaySound(ESoundType.PlayerHurt);
            
            if(_currentHealth <= 0) OnDie(false);
        }

        public void Heal(int value, ESoundType soundType = ESoundType.Heal)
        {
            value = PlayerManager.GetHealContextManager().GetHealContext(value, this).Value;
            if (value <= 0) return;
            
            AudioManager.PlaySound(soundType);
            
            IndicatorManager.SpawnIndicator(PlayerManager.PlayerPos, value, false, false);
            ParticleManager.SpawnParticles(EParticlesType.Heal, PlayerManager.PlayerPos);

            _currentHealth = Mathf.Clamp(_currentHealth + value, 0, MaxHealth);
            OnPlayerHeal?.Invoke(value, _currentHealth);
        }

        public override void OnDie(bool destroyObj = true, PoolManager poolManager = null)
        {
            AudioManager.PlaySound(ESoundType.PlayerDeath);
            PlayerManager.ManagePlayerDeath();
            
            base.OnDie(destroyObj);
        }
    }
}