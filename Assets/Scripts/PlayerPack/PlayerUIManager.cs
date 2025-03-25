using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpField;
        [SerializeField] private Image hpBar;
        [SerializeField] private TextMeshProUGUI expField;
        [SerializeField] private Image expBar;

        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        
        private void Awake()
        {
            PlayerHealth.OnPlayerDamaged += UpdateHp;
            PlayerHealth.OnPlayerHeal += UpdateHp;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= UpdateHp;
            PlayerHealth.OnPlayerHeal -= UpdateHp;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance);
            
            UpdateHp();
        }
        
        private void UpdateHp(int ignore)
        {
            UpdateHp();
        }

        private void UpdateHp()
        {
            hpField.text = $"{PlayerHealth.CurrentHealth}/{PlayerHealth.MaxHealth}";
            hpBar.fillAmount = (float)PlayerHealth.CurrentHealth / PlayerHealth.MaxHealth;
        }
    }
}