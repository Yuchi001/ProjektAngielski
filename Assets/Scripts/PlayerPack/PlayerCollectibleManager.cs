using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerCollectibleManager : MonoBehaviour
    {
        private readonly Dictionary<ECollectibleType, int> _collectiblesDict = new();

        private static PlayerCollectibleManager Instance => PlayerManager.PlayerCollectibleManager;

        public delegate void CollectibleModifyDelegate(ECollectibleType type, int current);
        public static event CollectibleModifyDelegate OnCollectibleModify;

        private void Awake()
        {
            foreach (ECollectibleType type in System.Enum.GetValues(typeof(ECollectibleType))) _collectiblesDict.Add(type, 0);
        }

        public static int GetCollectibleCount(ECollectibleType type)
        {
            return Instance._collectiblesDict[type];
        }

        public static bool HasCollectibleAmount(ECollectibleType type, int amount)
        {
            return Instance._collectiblesDict[type] >= amount;
        }

        public static void ModifyCollectibleAmount(ECollectibleType type, int amount)
        {
            var current = Instance._collectiblesDict[type] += amount;
            OnCollectibleModify?.Invoke(type, current);
        }

        public enum ECollectibleType
        {
            SOUL,
            COIN,
            SCRAP
        }
    }
}