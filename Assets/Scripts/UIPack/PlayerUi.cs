using System.Collections.Generic;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPack
{
    public class PlayerUi : MonoBehaviour
    {
        [SerializeField] private float uiUpdateRate = 3;
        [SerializeField] private Image expMeter;
        [SerializeField] private TextMeshProUGUI levelText;
        
        [SerializeField] private Image healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [SerializeField] private Image characterImage;
        [SerializeField] private Image characterFrameImage;

        [SerializeField] private GameObject dashStackHolder;

        [SerializeField] private GameObject dashStack;
        
        private static PlayerHealth PlayerHealth => PlayerManager.PlayerHealth;
        private static int PlayerMaxDashStacks => PlayerManager.PlayerMovement.MaxDashStacks;
        private static int CurrentPlayerDashStacks => PlayerManager.PlayerMovement.CurrentDashStacks;
        private static float DashProgress => PlayerManager.PlayerMovement.GetDashProgress();
        
        private float _uiUpdateTimer = 0;
        private List<Image> _spawnedDashStacks = new();

        private void Awake()
        {
            PlayerManager.OnPlayerReady += Setup;
            PlayerMovement.OnPlayerDashIncrement += UpdateDashStacks;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerReady -= Setup;
            PlayerMovement.OnPlayerDashIncrement -= UpdateDashStacks;
        }

        private void Setup()
        {
            healthBar.fillAmount = 1;
            expMeter.fillAmount = 1;
            levelText.text = "1 Lvl";
            healthText.text = $"{PlayerHealth.CurrentHealth}/{PlayerHealth.MaxHealth}";
            characterImage.sprite = PlayerManager.PickedCharacter.CharacterSprite;
            characterFrameImage.color = PlayerManager.PickedCharacter.CharacterColor;

            for (var i = 0; i < PlayerMaxDashStacks; i++)
            {
                var obj = Instantiate(dashStack, dashStackHolder.transform).GetComponent<Image>();
                _spawnedDashStacks.Add(obj);
                obj.fillAmount = 1;
            }
        }

        private void UpdateDashStacks(int value)
        {
            for (var i = 0; i < value; i++)
            {
                var obj = Instantiate(dashStack, dashStackHolder.transform).GetComponent<Image>();
                _spawnedDashStacks.Add(obj);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(dashStackHolder.GetComponent<RectTransform>());
        }

        private void Update()
        {
            if (!PlayerManager.HasInstance()) return;

            for (var i = 0; i < PlayerMaxDashStacks; i++) {
                if (i < CurrentPlayerDashStacks) _spawnedDashStacks[i].fillAmount = 1;
                else _spawnedDashStacks[i].fillAmount = i > CurrentPlayerDashStacks ? 
                    0 : DashProgress;
            }

            _uiUpdateTimer += Time.deltaTime;
            if (_uiUpdateTimer < 1 / uiUpdateRate) return;

            healthBar.fillAmount = PlayerHealth.CurrentHealth / (float)PlayerHealth.MaxHealth;
            healthText.text = $"{PlayerHealth.CurrentHealth}/{PlayerHealth.MaxHealth}";

            _uiUpdateTimer = 0;
        }
    }
}