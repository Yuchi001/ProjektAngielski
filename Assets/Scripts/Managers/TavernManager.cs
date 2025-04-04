using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainCameraPack;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using StructurePack.SO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class TavernManager : MonoBehaviour, IPersistentData
    {
        [SerializeField] private List<Transform> characterStructuresSpawnPositions;
        private static TavernManager Instance { get; set; }

        private List<SOCharacterStructure> _unlockedCharactersStructures = new();
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            if (!GameManager.HasInstance()) GameManager.LoadMenu();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(StructureManager.HasInstance);
            
            // TODO: render only unlocked characters
            var allCharacters = Resources.LoadAll<SOCharacterStructure>("Structures/Characters").ToList();

            var positions = characterStructuresSpawnPositions.Select(e => e.position).ToList();
            foreach (var character in allCharacters)
            {
                var randomIndex = Random.Range(0, positions.Count);
                var randomPos = positions[randomIndex];
                if (character.Is(PlayerManager.PickedCharacter)) PlayerManager.SetPosition(randomPos + new Vector3(0, 0.25f));
                else StructureManager.SpawnStructure(character, randomPos, transform);
                positions.Remove(randomPos);
            }
        }

        public static void LoadTavern()
        {
            MainCamera.SetFollow(PlayerManager.GetTransform());
        }

        public static void PickCharacter(SoCharacter character)
        {
            PlayerManager.ChangeCharacter(character);
        }

        public void OnLoadData(SaveManager.PlayerSaveData playerSaveData)
        {
            var allCharacters = Resources.LoadAll<SOCharacterStructure>("Structures/Characters").ToList();
            _unlockedCharactersStructures.Clear();
            foreach (var id in playerSaveData.currentCharactersIDs)
            {
                var found = allCharacters.FirstOrDefault(c => c.Is(id));
                if (found == default) continue;
                
                _unlockedCharactersStructures.Add(found);
            }
        }

        public void OnSaveData(ref SaveManager.PlayerSaveData playerSaveData)
        {
            playerSaveData.pickedCharacterID = PlayerManager.PickedCharacter.ID;
        }
    }
}