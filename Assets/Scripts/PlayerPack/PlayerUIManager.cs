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

        private static PlayerExp PlayerExp => PlayerManager.Instance.PlayerExp;
        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        
        private void Awake()
        {
            PlayerExp.OnGainExp += UpdateExp;
            PlayerHealth.OnPlayerDamaged += UpdateHp;
            PlayerHealth.OnPlayerHeal += UpdateHp;
        }

        private void OnDisable()
        {
            PlayerExp.OnGainExp -= UpdateExp;
            PlayerHealth.OnPlayerDamaged -= UpdateHp;
            PlayerHealth.OnPlayerHeal -= UpdateHp;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance);
            
            UpdateExp();
            UpdateHp();
        }

        private void UpdateExp()
        {
            expField.text = $"{PlayerExp.CurrentExp}/{PlayerExp.NextLevelExp}";
            expBar.fillAmount = (float)PlayerExp.CurrentExp / PlayerExp.NextLevelExp;
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