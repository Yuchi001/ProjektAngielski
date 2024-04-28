using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private float animTime = 0.1f;
        [SerializeField] private int levelOneCap;
        [SerializeField] private GameObject levelUpUiPrefab;

        public int CurrentLevel { get; private set; } = 1;
        public int NextLevelExp { get; private set; }
        public float CurrentExp { get; private set; }

        private List<float> _expQueue = new();

        private void Awake()
        {
            CurrentLevel = 1;
            NextLevelExp = levelOneCap;
        }

        public void GainExp(int expPoints)
        {
            CurrentExp += expPoints;
            if (CurrentExp >= NextLevelExp) StartCoroutine(LevelUp());
        }
        
        private IEnumerator LevelUp()
        {
            LeanTween.pause(gameObject);
            
            CurrentLevel++;
            var expExcess = CurrentExp - NextLevelExp;
            CurrentExp = 0;
            NextLevelExp = levelOneCap + Mathf.CeilToInt(CurrentLevel * (Mathf.Clamp(CurrentLevel - (float)CurrentLevel / 5, 1.1f, CurrentLevel)));

            var levelUpUiInstance = Instantiate(levelUpUiPrefab, GameManager.Instance.MainCanvas, false);
            levelUpUiInstance.GetComponent<LevelUpUi>().Setup();

            yield return new WaitUntil(() => levelUpUiInstance == null);
            
            if(expExcess >= 1) GainExp((int)expExcess);
        }
    }
}