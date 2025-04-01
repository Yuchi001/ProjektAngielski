using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Other;
using SavePack;
using StructurePack;
using StructurePack.SO;
using UnityEngine;

namespace MapGeneratorPack
{
    public class StructureGeneratorManager : MonoBehaviour
    {
        private List<SoStructure> _structures = new();

        private StructureBase _structurePrefab;
        
        private static StructureGeneratorManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            GameManager.OnGMStart += Init;
            GameManager.OnStartRun += Generate;
        }

        private void OnDisable()
        {
            GameManager.OnGMStart -= Init;
            GameManager.OnStartRun -= Generate;
        }

        private void Init()
        {
            _structures = Resources.LoadAll<SoStructure>("Structures").ToList();
            _structurePrefab = GameManager.GetPrefab<StructureBase>(PrefabNames.StructureBase);
        }

        public void Generate()
        {
            GenerateStructures();
        }

        private void GenerateStructures()
        {
            // TODO: Structure generation 
        }

        public static void SpawnStructure(SoStructure structure, Vector2 position)
        {
            Instantiate(Instance._structurePrefab, position, Quaternion.identity).Setup(structure);
        }

        public static SoStructure GetStructure(Func<SoStructure, bool> condition)
        {
            return Instance._structures.FirstOrDefault(condition);
        }
    }
}