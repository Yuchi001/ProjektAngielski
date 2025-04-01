using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using StructurePack.SO;
using UnityEngine;

namespace Managers
{
    public class MissionManager : MonoBehaviour, IPersistentData
    {
        [SerializeField] private List<Transform> characterStructuresSpawnPositions;
        [SerializeField] private Transform missionBoardSpawnPos;
        private static MissionManager Instance { get; set; }

        private List<SOCharacterStructure> _allCharacters = new();
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            _allCharacters = Resources.LoadAll<SOCharacterStructure>("Structures/Characters").ToList();
            var missionBoard = Resources.Load<SoMissionBoardStructure>("Structures/MissionBoard");
            
            StructureGeneratorManager.SpawnStructure(missionBoard, missionBoardSpawnPos.position);
        }

        public static void PickCharacter(SoCharacter character)
        {
            PlayerManager.Instance.ChangeCharacter(character);
        }

        public static void StartMission()
        {
            // TODO: start mission logic
            GameManager.StartRun();
        }

        public void OnLoadData(SaveManager.PlayerSaveData playerSaveData)
        {
            
        }

        public void OnSaveData(ref SaveManager.PlayerSaveData playerSaveData)
        {
            playerSaveData.pickedCharacterID = PlayerManager.Instance.PickedCharacter.ID;
        }
    }
}