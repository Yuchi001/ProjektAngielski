using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.Other;
using PlayerPack.Enums;
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
        [SerializeField] private TextMeshProUGUI scrapField;
        [SerializeField] private Image hpBar;
        [SerializeField] private RectTransform dashRect;

        private static int MaxHealth => PlayerManager.PlayerHealth.MaxHealth;

        private readonly List<PlayerManager.State> _activeStates = new()
        {
            PlayerManager.State.IN_MAP,
            PlayerManager.State.ON_MISSION,
            PlayerManager.State.IN_TAVERN
        };

        private Image _dashStackPrefab;
        private readonly List<Image> _spawnedDashImages = new();
        
        private void Awake()
        {
            PlayerManager.OnPlayerReady += PrepareUI;
            
            _dashStackPrefab = GameManager.GetPrefab<Image>(PrefabNames.DashUI);
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            PlayerHealth.OnPlayerDamaged += UpdateHp;
            PlayerHealth.OnPlayerHeal += UpdateHp;
            PlayerManager.OnChangeState += OnChangeState;
            PlayerCollectibleManager.OnCollectibleModify += UpdateCollectibles;
        }
        

        public void PrepareUI()
        {
            UpdateHp(PlayerManager.PlayerHealth.CurrentHealth, MaxHealth);
            foreach (PlayerCollectibleManager.ECollectibleType type in System.Enum.GetValues(typeof(PlayerCollectibleManager.ECollectibleType)))
            {
                UpdateCollectibles(type, PlayerCollectibleManager.GetCollectibleCount(type));
            }
            SpawnDashStacks();
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= UpdateHp;
            PlayerHealth.OnPlayerHeal -= UpdateHp;
            PlayerCollectibleManager.OnCollectibleModify -= UpdateCollectibles;
            PlayerManager.OnChangeState -= OnChangeState;
            PlayerManager.OnPlayerReady -= PrepareUI;
        }
        
        private void OnChangeState(PlayerManager.State state)
        {
            var active = _activeStates.Contains(state);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }

            var isOnMission = state == PlayerManager.State.ON_MISSION;
            scrapField.gameObject.SetActive(isOnMission);
            soulField.gameObject.SetActive(isOnMission);
            coinField.gameObject.SetActive(isOnMission);
        }

        private void UpdateHp(int value, int current)
        {
            hpField.text = $"{current}/{MaxHealth}";
            hpBar.fillAmount = (float)current / MaxHealth;
        }

        private void UpdateCollectibles(PlayerCollectibleManager.ECollectibleType type, int current)
        {
            switch (type)
            {
                case PlayerCollectibleManager.ECollectibleType.SOUL:
                    soulField.text = $"<sprite name=\"souls\"> x{current}";
                    break;
                case PlayerCollectibleManager.ECollectibleType.COIN:
                    coinField.text = $"<sprite name=\"coins\"> x{current}";
                    break;
                case PlayerCollectibleManager.ECollectibleType.SCRAP:
                    scrapField.text = $"<sprite name=\"scraps\"> x{current}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, $"Collectible of type {type} not handled in PlayerUIManager!");
            }
        }

        private void Update()
        {
            if (PlayerManager.PlayerMovement.CurrentDashStacks == PlayerManager.PlayerMovement.MaxDashStacks || !PlayerMovement.CanDash()) return;
            UpdateDashStacks();
        }

        private void SpawnDashStacks()
        {
            _spawnedDashImages.Clear();
            foreach (Transform child in dashRect) Destroy(child.gameObject);
            var startingDashStacks = PlayerManager.PlayerMovement.MaxDashStacks;
            for (var i = 0; i < startingDashStacks; i++) _spawnedDashImages.Add(Instantiate(_dashStackPrefab, dashRect));
            LayoutRebuilder.ForceRebuildLayoutImmediate(dashRect);
        }

        private void UpdateDashStacks()
        {
            var current = PlayerManager.PlayerMovement.CurrentDashStacks;
            var childCount = dashRect.childCount;
            var maxDashStacks = PlayerManager.PlayerMovement.MaxDashStacks;
            if (childCount != maxDashStacks) SpawnDashStacks();
            
            for (var i = 0; i < maxDashStacks; i++)
            {
                var child = _spawnedDashImages[i];
                
                if (i > current)
                {
                    child.fillAmount = 0;
                    continue;
                }

                if (i == current)
                {
                    child.fillAmount = PlayerManager.PlayerMovement.GetDashProgress();
                    continue;
                }

                child.fillAmount = 1;
            }
        }
    }
}