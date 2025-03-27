using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpField;
        [SerializeField] private TextMeshProUGUI coinField;
        [SerializeField] private TextMeshProUGUI soulField;
        [SerializeField] private Image hpBar;

        private static int MaxHealth => PlayerManager.Instance.PlayerHealth.MaxHealth;
        private static int CoinCount => PlayerManager.Instance.PlayerCoinManager.GetCurrentCount();
        
        private void Awake()
        {
            PlayerHealth.OnPlayerDamaged += UpdateHp;
            PlayerHealth.OnPlayerHeal += UpdateHp;
            PlayerSoulManager.OnSoulCountChange += UpdateSoulCount;
            PlayerCoinManager.OnCoinCountChange += UpdateCoinCount;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= UpdateHp;
            PlayerHealth.OnPlayerHeal -= UpdateHp;
            PlayerSoulManager.OnSoulCountChange -= UpdateSoulCount;
            PlayerCoinManager.OnCoinCountChange -= UpdateCoinCount;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance);
            
            UpdateHp(0, MaxHealth);
            UpdateSoulCount(0, 0);
            UpdateCoinCount(0, CoinCount);
        }
        
        private void UpdateHp(int value, int current)
        {
            hpField.text = $"{current}/{MaxHealth}";
            hpBar.fillAmount = (float)current / MaxHealth;
        }

        private void UpdateSoulCount(int value, int current)
        {
            coinField.text = $"x{current}";
        }

        private void UpdateCoinCount(int value, int current)
        {
            soulField.text = $"x{current}";
        }
    }
}