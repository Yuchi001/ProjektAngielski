using System;
using System.Collections;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUi : MonoBehaviour
    {
        [SerializeField] private float uiUpdateRate = 3;
        [SerializeField] private Image expMeter;
        [SerializeField] private TextMeshProUGUI levelText;
        
        [SerializeField] private Image healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [SerializeField] private Image characterImage;
        
        private static PlayerExp PlayerExp => PlayerManager.Instance.PlayerExp;
        private static PlayerHealth PlayerHealth => PlayerManager.Instance.PlayerHealth;
        
        private float _uiUpdateTimer = 0;

        private void Awake()
        {
            PlayerManager.OnPlayerReady += Setup;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerReady -= Setup;
        }

        private void Setup()
        {
            healthBar.fillAmount = 1;
            expMeter.fillAmount = 1;
            levelText.text = "1 Lvl";
            healthText.text = $"{PlayerHealth.CurrentHealth}/{PlayerHealth.MaxHealth}";
            characterImage.sprite = PlayerManager.Instance.PickedCharacter.CharacterSprite;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            _uiUpdateTimer += Time.deltaTime;
            if (_uiUpdateTimer < 1 / uiUpdateRate) return;

            healthBar.fillAmount = PlayerHealth.CurrentHealth / (float)PlayerHealth.MaxHealth;
            healthText.text = PlayerHealth.CurrentHealth.ToString();

            expMeter.fillAmount = PlayerExp.CurrentExp / PlayerExp.NextLevelExp;
            levelText.text = PlayerExp.CurrentLevel + " LvL";
            _uiUpdateTimer = 0;
        }
    }
}