using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerExp : MonoBehaviour
    {
        [SerializeField] private int levelOneCap;

        public int StackedLevels { get; private set; } = 1;
        public int NextLevelExp { get; private set; }
        public int CurrentExp { get; private set; }

        public delegate void GainExpDelegate();
        public static event GainExpDelegate OnGainExp;
        
        private void Awake()
        {
            StackedLevels = 1;
            NextLevelExp = levelOneCap;
        }

        public void GainExp(int expPoints)
        {
            CurrentExp += expPoints;
            OnGainExp?.Invoke();
            if (CurrentExp >= NextLevelExp) LevelUp();
        }
        
        private void LevelUp()
        {
            StackedLevels++;
            var expExcess = CurrentExp - NextLevelExp;
            CurrentExp = 0;
            NextLevelExp = levelOneCap * StackedLevels + StackedLevels * StackedLevels;

            if(expExcess >= 1) GainExp((int)expExcess);
        }
    }
}