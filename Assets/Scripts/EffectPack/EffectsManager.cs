using System;
using System.Collections.Generic;
using EffectPack.SO;
using GameLoaderPack;
using Managers;
using Managers.Other;
using MapPack;
using Other;
using Other.Enums;
using PlayerPack.Decorators;
using UnityEngine;

//TODO: Optimize effects manager for pool mechanism
namespace EffectPack
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] private Transform statusesHolder;
        [SerializeField] private CanBeDamaged _canBeDamaged;
        
        private readonly Dictionary<EEffectType, EffectStatusObject> _statuses = new();

        public bool Stunned => HasEffect(EEffectType.Stun);
        public bool Slowed => HasEffect(EEffectType.Slow);

        private IEnumerable<SoEffectBase> _effects;

        public void OnCreate()
        {
            _effects = Resources.LoadAll<SoEffectBase>("EffectStatus");

            var effectStatusPrefab = GameManager.GetPrefab<EffectStatusObject>(PrefabNames.EffectStatus);
            foreach (var effectType in (EEffectType[])System.Enum.GetValues(typeof(EEffectType)))
            {
                if (effectType == EEffectType.None) continue;
                
                var prefab = Instantiate(effectStatusPrefab, statusesHolder);
                var effect = GetEffect(effectType);
                if (effect == null) throw new Exception($"Effect of type {effectType} not found in effects dict");
                prefab.SpawnSetup(GetEffect(effectType), this, _canBeDamaged);
                _statuses.Add(effectType, prefab);
            }
        }
        
        private SoEffectBase GetEffect(EEffectType effectType)
        {
            foreach (var effectBase in _effects) if (effectBase.EffectType == effectType) return effectBase;

            return null;
        }

        public void Setup(CanBeDamaged canBeDamaged)
        {
            _canBeDamaged = canBeDamaged;
            _canBeDamaged.SpriteRenderer.color = Color.white;
            foreach (var status in _statuses.Values) status.Setup();
        }

        public void AddEffect(EffectContext effectContext)
        {
            if (effectContext.EffectType == EEffectType.None) return;
            _statuses[effectContext.EffectType].StackEffect(effectContext);
        }

        public bool HasEffect(EEffectType effectType)
        {
            return _statuses[effectType].IsActive;
        }
        
        public int GetEffectStacks(EEffectType effectType)
        {
            return _statuses[effectType].IsActive ? _statuses[effectType].CurrentStacks : 0;
        }

        public bool TryGetAnyEffect(out SoEffectBase returnedEffect)
        {
            returnedEffect = null;
            foreach (var effect in _statuses.Values)
            {
                if (!effect.IsActive) continue;

                returnedEffect = effect.EffectBase;
                return true;
            }
            return false;
        }
    }
}