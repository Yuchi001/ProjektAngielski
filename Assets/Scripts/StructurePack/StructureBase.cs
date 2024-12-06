using System;
using System.Collections;
using Managers;
using StructurePack.SO;
using UnityEngine;

namespace StructurePack
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class StructureBase : MonoBehaviour
    {
        private GameObject _spawnedInteractionUiPrefab;
        private SoStructure _structureData;
        private CircleCollider2D Collider => GetComponent<CircleCollider2D>();

        private static readonly float COLLISION_RANGE = 2;
        private static readonly string PREFAB_NAME = "StructureInteractionPrefab";

        private bool _inRange = false;

        protected bool _toggle = false;
        protected bool _wasUsed = false;
        
        private void Awake()
        {
            Collider.radius = COLLISION_RANGE;
        }

        public void SpawnStructure(SoStructure structureData)
        {
            _structureData = structureData;
            
            var prefab = GameManager.Instance.GetPrefab(PREFAB_NAME);
            _spawnedInteractionUiPrefab = Instantiate(prefab, transform);
            _spawnedInteractionUiPrefab.transform.position = Vector3.zero;
            _spawnedInteractionUiPrefab.SetActive(false);
        }

        private void Update()
        {
            if (!_inRange || (_wasUsed && !_structureData.Reusable) || !Input.GetKey(KeyCode.E)) return;

            _wasUsed = true;
            _toggle = !_toggle;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = true;
            _spawnedInteractionUiPrefab.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = false;
            _spawnedInteractionUiPrefab.SetActive(false);
        }
    }
}