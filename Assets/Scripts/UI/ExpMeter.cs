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
        [SerializeField] private float updateRate = 3;
        [SerializeField] private Image meter;
        [SerializeField] private TextMeshProUGUI levelText;

        private PlayerExp PlayerExp => PlayerManager.Instance.PlayerExp;

        private float _timer = 0;
        
        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            meter.fillAmount = PlayerExp.CurrentExp / PlayerExp.NextLevelExp;

            _timer += Time.deltaTime;
            if (_timer < 1 / updateRate) return;
            
            levelText.text = PlayerExp.CurrentLevel + " LvL";
            _timer = 0;
        }
    }
}