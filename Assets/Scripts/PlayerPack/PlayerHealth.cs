using Managers;
using Managers.Enums;
using Other;
using PlayerPack.SO;
using UI;
using UnityEngine;
using WeaponPack.Other;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
        [SerializeField] private float zoneCheckPerSec = 4;
        [SerializeField] private float healPerSec = 0.1f;
        [SerializeField] private GameObject healParticles;
        [SerializeField] private GameObject damageIndicator;

        public override int MaxHealth => PickedCharacter.MaxHp;
        public override int CurrentHealth => _currentHealth;
        
        private int _currentHealth = 0;

        private float _zoneTimer = 0;
        private float _healTimer = 0;

        public bool Invincible { get; set; } = false;

        public delegate void PlayerDamagedDelegate();
        public static event PlayerDamagedDelegate OnPlayerDamaged; 
        
        private static SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;

        private void OnEnable()
        {
            _currentHealth = PickedCharacter.MaxHp;
        }

        protected override void OnUpdate()
        {
            ManageZone();
            ManageAutoHeal();
        }

        private void ManageAutoHeal()
        {
            if (_currentHealth >= MaxHealth) return;
            
            _healTimer += Time.deltaTime;
            if (_healTimer < 1f / healPerSec) return;
            _healTimer = 0;
            
            Heal(MaxHealth / 10);
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

        public void Heal(int value)
        {
            AudioManager.Instance.PlaySound(ESoundType.Heal);
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicator, value, false);
            
            var particles = Instantiate(healParticles, transform.position, Quaternion.identity);
            Destroy(particles, 1f);
            
            _currentHealth = Mathf.Clamp(_currentHealth + value, 
                0, PickedCharacter.MaxHp);
        }

        public override void OnDie(bool destroyObj = true)
        {
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
    }
}