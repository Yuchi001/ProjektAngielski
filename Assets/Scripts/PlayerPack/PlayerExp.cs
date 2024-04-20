using System;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private int levelOneCap;

        public int CurrentLevel { get; private set; } = 1;
        public int NextLevelExp { get; private set; }
        public int CurrentExp { get; private set; }

        private void Awake()
        {
            CurrentLevel = 1;
            NextLevelExp = levelOneCap;
        }

        public void GainExp(int expPoints)
        {
            CurrentExp += expPoints;
            if (CurrentExp >= NextLevelExp) LevelUp();
        }

        private void LevelUp()
        {
            CurrentLevel++;
            var expExcess = CurrentExp - NextLevelExp;
            CurrentExp = 0;
            NextLevelExp = levelOneCap + Mathf.CeilToInt(CurrentLevel * (Mathf.Clamp(CurrentLevel - (float)CurrentLevel / 10, 1.1f, CurrentLevel)));
            if(expExcess > 0) GainExp(expExcess);
        }
    }
}