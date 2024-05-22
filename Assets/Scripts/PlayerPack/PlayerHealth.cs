using System;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack.SO;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
        [SerializeField] private GameObject healParticles;
        [SerializeField] private Image healthMeter;
        [SerializeField] private GameObject damageIndicator;

        private int _currentHealth = 0;
        
        private static SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;

        private void OnEnable()
        {
            _currentHealth = PickedCharacter.MaxHp;
        }

        protected override void OnUpdate()
        {
            healthMeter.fillAmount = (float)_currentHealth / PickedCharacter.MaxHp;
        }

        public override void GetDamaged(int value)
        {
            if (Dead) return;
            
            base.GetDamaged(value);
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
    }
}