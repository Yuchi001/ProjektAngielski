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
        }

        private void OnGainExp(int expPoints)
        {
            CurrentExp += expPoints;
            if (CurrentExp >= NextLevelExp) LevelUp();
        }

        private void LevelUp()
        {
            CurrentLevel++;
            CurrentExp -= NextLevelExp;
            NextLevelExp = levelOneCap + Mathf.CeilToInt(CurrentLevel * (Mathf.Clamp(CurrentLevel - (float)CurrentLevel / 10, 1.1f, CurrentLevel)));
        }
    }
}