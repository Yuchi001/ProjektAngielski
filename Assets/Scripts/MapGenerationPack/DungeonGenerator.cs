using System;
using System.Collections.Generic;
using PlayerPack;
using UnityEngine;

namespace MapGenerationPack
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> tilemapPrefabs = new();

        private List<GameObject> spawnedTiles = new();

        private Vector2 PlayerPos => PlayerManager.Instance.transform.position;

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }
    }
}