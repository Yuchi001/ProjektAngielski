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
        [SerializeField] private int levelOneCap;
        [SerializeField] private GameObject levelUpUiPrefab;

        public int CurrentLevel { get; private set; } = 1;
        public int NextLevelExp { get; private set; }
        public float CurrentExp { get; private set; }
        
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
            NextLevelExp = levelOneCap * CurrentLevel + CurrentLevel * CurrentLevel;

            var levelUpUiInstance = Instantiate(levelUpUiPrefab, GameUiManager.Instance.GameCanvas, false);
            //levelUpUiInstance.GetComponent<LevelUpUi>().Setup(); TODO: Refactor

            yield return new WaitUntil(() => levelUpUiInstance == null);
            
            if(expExcess >= 1) GainExp((int)expExcess);
        }
    }
}