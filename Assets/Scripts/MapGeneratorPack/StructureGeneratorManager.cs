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

            var structurePrefab = GameManager.Instance.GetPrefab<StructureBase>(PrefabNames.StructureBase);
            for (var i = 0; i < 5; i++)
            {
                foreach (var structure in _structures)
                {
                    var randomOffsetX = Random.Range(-3f, 3f);
                    var randomOffsetY = Random.Range(-3f, 3f);
                    var newPos = transform.position;
                    newPos.x = randomOffsetX;
                    newPos.y = randomOffsetY;
                    Instantiate(structurePrefab, newPos, Quaternion.identity).Setup(structure);
                }
            }
        }
    }
}