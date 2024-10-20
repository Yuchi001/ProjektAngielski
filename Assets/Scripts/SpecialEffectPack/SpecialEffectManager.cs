using System;
using System.Collections.Generic;
using SpecialEffectPack.Enums;
using UnityEngine;

namespace SpecialEffectPack
{
    public class SpecialEffectManager : MonoBehaviour
    {
        #region Singleton

        public static SpecialEffectManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        [SerializeField] private List<ExplosionEffectData> explosionEffects = new();

        private readonly Dictionary<ESpecialEffectType, GameObject> _effectDictionary = new();
        
        private void Start()
        {
           explosionEffects.ForEach(e => _effectDictionary.Add(e.SpecialEffectType, e.ExplosionPrefab));            
        }

        public ExplosionAnimation SpawnExplosion(ESpecialEffectType specialEffectType, Vector2 position, float range)
        {
            if (_effectDictionary.TryGetValue(specialEffectType, out var value))
            {
                var explosion = Instantiate(value, position, Quaternion.identity);
                return explosion.GetComponent<ExplosionAnimation>().Trigger(range);
            }
            
            Debug.LogError($"{specialEffectType} has not been defined!");
            return null;
        }

        [System.Serializable]
        public struct ExplosionEffectData
        {
            [SerializeField] private ESpecialEffectType specialEffectType;
            [SerializeField] private GameObject explosionPrefab;

            public ESpecialEffectType SpecialEffectType => specialEffectType;
            public GameObject ExplosionPrefab => explosionPrefab;
        }
    }
}