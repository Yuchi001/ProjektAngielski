using System;
using System.Linq;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using UnityEngine;

namespace Managers
{
    public class MissionManager : MonoBehaviour, IPersistentData
    {
        [SerializeField] private SoCharacter defaultCharacter;
        private static MissionManager Instance { get; set; }

        private SoCharacter _pickedCharacter;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void PickCharacter(SoCharacter character)
        {
            // TODO: Pick character logic
            _pickedCharacter = character;
        }

        public void StartMission()
        {
            // TODO: start mission logic
            GameManager.StartRun(_pickedCharacter);
        }

        public void OnLoadData(SaveManager.PlayerSaveData playerSaveData)
        {
            var allCharacters = Resources.LoadAll<SoCharacter>("Characters");
            _pickedCharacter = allCharacters.FirstOrDefault(p => p.CharacterName == playerSaveData.pickedCharacterID) ?? defaultCharacter;
        }

        public void OnSaveData(ref SaveManager.PlayerSaveData playerSaveData)
        {
            playerSaveData.pickedCharacterID = PlayerManager.Instance.PickedCharacter.CharacterName;
        }
    }
}