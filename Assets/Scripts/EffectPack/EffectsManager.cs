using System.Collections.Generic;
using Managers;
using Managers.Other;
using Other;
using Other.Enums;
using UnityEngine;

namespace EffectPack
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] private Transform statusesHolder;
        
        private readonly Dictionary<EEffectType, EffectStatusObject> _statuses = new();

        public bool Stuned => HasEffect(EEffectType.Stun);
        public bool Slowed => HasEffect(EEffectType.Slow);

        private CanBeDamaged _canBeDamaged;
        
        private void Start()
        {
            var effectStatusPrefab = GameManager.Instance.GetPrefab<EffectStatusObject>(PrefabNames.EffectStatus);
            foreach (var effectType in (EEffectType[])System.Enum.GetValues(typeof(EEffectType)))
            {
                var prefab = Instantiate(effectStatusPrefab, statusesHolder);
                prefab.SpawnSetup(effectType, this, _canBeDamaged);
                _statuses.Add(effectType, prefab);   
            }
        }

        public void Setup(CanBeDamaged canBeDamaged)
        {
            _canBeDamaged = canBeDamaged;
            foreach (var status in _statuses.Values) status.Setup();
        }

        public void AddEffect(EEffectType effectType, float time)
        {
            _statuses[effectType].StackEffect(time);
        }

        public bool HasEffect(EEffectType effectType)
        {
            return _statuses[effectType].isActiveAndEnabled;
        }
    }
}