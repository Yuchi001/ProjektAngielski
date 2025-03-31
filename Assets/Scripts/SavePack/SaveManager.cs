using System;
using System.Collections.Generic;
using PlayerPack.SO;
using UnityEngine;

namespace SavePack
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }


        public class SaveData
        {
            public List<string> currentCharactersIDs;
            public string pickedCharacterID;
            public List<string> seenEnemiesIDs;
            public List<string> seenWeaponsIDs;
        }
    }
}