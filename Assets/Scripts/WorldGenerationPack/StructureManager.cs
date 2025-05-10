using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Other;
using StructurePack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldGenerationPack
{
    public class StructureManager : MonoBehaviour, IMainSingleton
    {
        private List<SoStructure> _structures = new();

        private StructureBase _structurePrefab;
        
        private static StructureManager Instance { get; set; }

        private StructureInteractionHandler _interactionHandler;

        public static bool HasInstance() => Instance != null;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void Init()
        {
            _structures = Resources.LoadAll<SoStructure>("Structures").ToList();
            _structurePrefab = GameManager.GetPrefab<StructureBase>(PrefabNames.StructureBase);
            _interactionHandler = new StructureInteractionHandler();
        }

        private void Update()
        {
            _interactionHandler.HandleQueue();
        }

        public static void AddToQueue(StructureBase structureBase)
        {
            Instance._interactionHandler.AddToQueue(structureBase);
        }

        public static void RemoveFromQueue(StructureBase structureBase)
        {
            Instance._interactionHandler.RemoveFromQueue(structureBase.GetInstanceID());
        }

        public static bool IsFocused(StructureBase structureBase)
        {
            return Instance._interactionHandler.FocusedStructure == structureBase.GetInstanceID();
        }

        public static StructureBase SpawnStructure(SoStructure structure, Vector2 position)
        {
            return Instantiate(Instance._structurePrefab, position, Quaternion.identity).Setup(structure);
        }
        
        public static StructureBase SpawnStructure(SoStructure structure, Vector2 position, Scene scene)
        {
            var spawnedStructure = Instantiate(Instance._structurePrefab, position, Quaternion.identity).Setup(structure);
            SceneManager.MoveGameObjectToScene(spawnedStructure.gameObject, scene);
            return spawnedStructure;
        }
        
        public static StructureBase SpawnStructure(SoStructure structure, Vector2 position, GameManager.EScene sceneType)
        {
            var scene = SceneManager.GetSceneByBuildIndex((int)sceneType);
            var spawnedStructure = Instantiate(Instance._structurePrefab, position, Quaternion.identity).Setup(structure);
            SceneManager.MoveGameObjectToScene(spawnedStructure.gameObject, scene);
            return spawnedStructure;
        }

        public static SoStructure GetStructure(Func<SoStructure, bool> condition)
        {
            return Instance._structures.FirstOrDefault(condition);
        }
    }
}