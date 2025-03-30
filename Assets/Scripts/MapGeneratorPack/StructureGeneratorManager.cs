using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Other;
using StructurePack;
using StructurePack.SO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorPack
{
    public class StructureGeneratorManager : MonoBehaviour
    {
        private List<SoStructure> _structures = new();

        private void Start()
        {
            _structures = Resources.LoadAll<SoStructure>("Structures").ToList();
            Generate();
        }

        public void Generate()
        {
            GenerateStructures();
        }

        private void GenerateStructures()
        {
            // TODO: Structure generation 
        }
    }
}