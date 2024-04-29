using System;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class ExpMeter : MonoBehaviour
    {
        [SerializeField] private Image meter;
        [SerializeField] private TextMeshProUGUI levelText;

        private PlayerExp PlayerExp => PlayerManager.Instance.PlayerExp;
        
        private void Update()
        {
            if (PlayerExp == null) return;

            meter.fillAmount = PlayerExp.CurrentExp / PlayerExp.NextLevelExp;
            levelText.text = PlayerExp.CurrentLevel + " LvL";
        }
    }
}