using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapGeneratorPack;
using NUnit.Framework;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    public class MissionManager : MonoBehaviour, IPersistentData
    {
        [SerializeField] private List<Transform> characterStructuresSpawnPositions;
        [SerializeField] private Transform missionBoardSpawnPos;
        private static MissionManager Instance { get; set; }

        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            if (!GameManager.HasInstance()) GameManager.LoadGameScene();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(StructureManager.HasInstance);
            
            var allCharacters = Resources.LoadAll<SOCharacterStructure>("Structures/Characters").ToList();
            var missionBoard = Resources.Load<SoMissionBoardStructure>("Structures/MissionBoard");

            StructureManager.SpawnStructure(missionBoard, missionBoardSpawnPos.position, transform);
            var positions = characterStructuresSpawnPositions.Select(e => e.position).ToList();
            foreach (var character in allCharacters)
            {
                if (character.Is(PlayerManager.Instance.PickedCharacter)) continue;
                var randomIndex = Random.Range(0, positions.Count);
                var randomPos = positions[randomIndex];
                StructureManager.SpawnStructure(character, randomPos, transform);
                positions.Remove(randomPos);
            }
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