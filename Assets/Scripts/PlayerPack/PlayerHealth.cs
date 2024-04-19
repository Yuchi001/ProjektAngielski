using System;
using PlayerPack.SO;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private Image healthMeter;

        private int _currentHealth = 0;
        
        private static SoCharacter PickedCharacter => PlayerManager.Instance.PickedCharacter;

        private void Start()
        {
            _currentHealth = PickedCharacter.MaxHp;
        }

        private void Update()
        {
            healthMeter.fillAmount = (float)_currentHealth / PickedCharacter.MaxHp;
        }
    }
}