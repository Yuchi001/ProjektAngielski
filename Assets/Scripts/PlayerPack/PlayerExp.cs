using System;
using System.Collections;
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

        private void Awake()
        {
            CurrentLevel = 1;
            NextLevelExp = levelOneCap;
        }

        public void GainExp(int expPoints)
        {
            LeanTween.value(CurrentExp, CurrentExp + expPoints, animTime).setOnUpdate((value) =>
            {
                CurrentExp = value;
                if (CurrentExp >= NextLevelExp) StartCoroutine(LevelUp());
                LeanTween.pause(gameObject);
            });
        }
        
        private IEnumerator LevelUp()
        {
            CurrentLevel++;
            var expExcess = CurrentExp - NextLevelExp;
            CurrentExp = 0;
            NextLevelExp = levelOneCap + Mathf.CeilToInt(CurrentLevel * (Mathf.Clamp(CurrentLevel - (float)CurrentLevel / 10, 1.1f, CurrentLevel)));

            var levelUpUiInstance = Instantiate(levelUpUiPrefab, GameManager.Instance.MainCanvas, true);
            levelUpUiInstance.GetComponent<LevelUpUi>().Setup();

            yield return new WaitUntil(() => levelUpUiInstance == null);
            
            if(expExcess > 0) GainExp((int)expExcess);
        }
    }
}