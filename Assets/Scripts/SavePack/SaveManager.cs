using System;
using System.Collections.Generic;
using System.Linq;
using PlayerPack.SO;
using UnityEngine;

namespace SavePack
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager Instance { get; set; }

        private PlayerSaveData _data;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public static PlayerSaveData LoadData()
        {
            Instance._data = FileManager.GetPlayerData();
            foreach (var obj in GetSubscribers())
            {
                obj.OnLoadData(Instance._data);
            }

            return Instance._data;
        }

        public static PlayerSaveData SaveData()
        {
            foreach (var obj in GetSubscribers())
            {
                obj.OnSaveData(ref Instance._data);
            }
            FileManager.SavePlayerData(Instance._data);
            return Instance._data;
        }

        private static IEnumerable<IPersistentData> GetSubscribers()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPersistentData>();
        }

        public class PlayerSaveData
        {
            public List<string> currentCharactersIDs = new();
            public string pickedCharacterID = "";
            public List<string> seenEnemiesIDs = new();
            public List<string> seenWeaponsIDs = new();
        }
    }
}