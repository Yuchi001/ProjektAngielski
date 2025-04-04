using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.Other;
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
        [SerializeField] private RectTransform dashRect;

        private static int MaxHealth => PlayerManager.PlayerHealth.MaxHealth;
        private static int CoinCount => PlayerManager.PlayerCoinManager.GetCurrentCount();

        private readonly List<PlayerManager.State> _activeStates = new()
        {
            PlayerManager.State.IN_MAP,
            PlayerManager.State.ON_MISSION,
            PlayerManager.State.IN_TAVERN
        };

        private Image _dashStackPrefab;
        
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.PlayerStatsManager != null);
            
            gameObject.SetActive(false);
            PlayerHealth.OnPlayerDamaged += UpdateHp;
            PlayerHealth.OnPlayerHeal += UpdateHp;
            PlayerSoulManager.OnSoulCountChange += UpdateSoulCount;
            PlayerCoinManager.OnCoinCountChange += UpdateCoinCount;
            PlayerManager.OnChangeState += OnChangeState;
            PlayerMovement.OnPlayerDash += UpdateDashStacks;
            
            UpdateHp(0, MaxHealth);
            UpdateSoulCount(0, 0);
            UpdateCoinCount(0, CoinCount);

            _dashStackPrefab = GameManager.GetPrefab<Image>(PrefabNames.DashUI);
            var startingDashStacks = PlayerManager.PlayerMovement.MaxDashStacks;
            for (var i = 0; i < startingDashStacks; i++) Instantiate(_dashStackPrefab, dashRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(dashRect);
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= UpdateHp;
            PlayerHealth.OnPlayerHeal -= UpdateHp;
            PlayerSoulManager.OnSoulCountChange -= UpdateSoulCount;
            PlayerCoinManager.OnCoinCountChange -= UpdateCoinCount;
            PlayerManager.OnChangeState -= OnChangeState;
            PlayerMovement.OnPlayerDash -= UpdateDashStacks;
        }
        
        private void OnChangeState(PlayerManager.State state)
        {
            var active = _activeStates.Contains(state);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
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

        private void UpdateDashStacks(int current)
        {
            for (var i = 0; i < dashRect.childCount; i++)
            {
                var child = dashRect.GetChild(i).GetComponent<Image>();
                
                if (i + 1 > current)
                {
                    child.fillAmount = 0;
                    continue;
                }

                if (i + 1 == current)
                {
                    child.fillAmount = PlayerManager.PlayerMovement.GetDashProgress();
                    continue;
                }

                child.fillAmount = 1;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(dashRect);
        }
    }
}