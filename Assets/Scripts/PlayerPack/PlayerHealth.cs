using System;
using Other;
using PlayerPack.SO;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class PlayerHealth : CanBeDamaged
    {
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
            base.GetDamaged(value);
            _currentHealth = Mathf.Clamp(_currentHealth - value, 
                0, PickedCharacter.MaxHp);
        }

        public void Heal(int value)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + value, 
                0, PickedCharacter.MaxHp);
            
            DamageIndicator.SpawnDamageIndicator(transform.position, damageIndicator, value, false);
        }
    }
}