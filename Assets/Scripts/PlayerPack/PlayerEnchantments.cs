using System;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerEnchantments : MonoBehaviour
    {
        private readonly Dictionary<EEnchantmentName, EnchantmentBase> _currentEnchantments = new();
        private List<SoEnchantment> _allEnchantments = new();
        private List<EEnchantmentName> _enchantmentValues = new();
        private List<string> _enchantmentNames = new();
        private readonly Dictionary<string, EEnchantmentName> _enchantmentMap = new();

        private List<SoEnchantment> _unusedEnchantments = new();
        
        public delegate void AddEnchantmentDelegate(SoEnchantment enchantment, EnchantmentBase logic);
        public static event AddEnchantmentDelegate OnAddEnchantment;
        
        public delegate void RemoveEnchantmentDelegate(EEnchantmentName enchantmentName);
        public static event RemoveEnchantmentDelegate OnRemoveEnchantment;
        private Dictionary<EEnchantmentName, Dictionary<EValueKey, float>> EnchantmentValueDictionary { get; } = new();
        
        private void Awake()
        {
            _enchantmentValues = ((EEnchantmentName[])Enum.GetValues(typeof(EEnchantmentName))).ToList();
            _enchantmentNames = Enum.GetNames(typeof(EEnchantmentName)).ToList();
            _allEnchantments = Resources.LoadAll<SoEnchantment>("Enchantments").Select(Instantiate).ToList();
            _unusedEnchantments = new List<SoEnchantment>(_allEnchantments);
            
            for (var i = 0; i < _enchantmentValues.Count; i++)
            {
                _enchantmentMap.Add(_enchantmentNames[i], _enchantmentValues[i]);
            }
            
            foreach (var enchantment in _allEnchantments)
            {
                var enchantmentParams = enchantment.EnchantmentParams.ToDictionary(param => param.Key, param => param.Value);
                EnchantmentValueDictionary.Add(enchantment.EnchantmentName, enchantmentParams);
            }
        }

        public float GetParamValue(EEnchantmentName enchantmentName, EValueKey paramName)
        {
            return EnchantmentValueDictionary[enchantmentName][paramName];
        }
        
        public void AddEnchantment(SoEnchantment enchantmentToAdd)
        {
            var alreadyIn = _currentEnchantments.ContainsKey(enchantmentToAdd.EnchantmentName);
            if (alreadyIn) return;
            
            if (enchantmentToAdd.HasRequirement)
            {
                var requirementValid = _currentEnchantments.ContainsKey(enchantmentToAdd.Requirement);
                if (requirementValid) RemoveEnchantment(enchantmentToAdd.Requirement);
            }

            var enchantmentLogic = enchantmentToAdd.Setup(transform);
            _currentEnchantments.Add(enchantmentToAdd.EnchantmentName, enchantmentLogic);
            
            OnAddEnchantment?.Invoke(enchantmentToAdd, enchantmentLogic);
        }

        /// <summary>
        /// Get random enchantment list.
        /// </summary>
        /// <param name="count">How long enchantment list should be</param>
        /// <returns>List of unique enchantments. In case of any errors, return empty list.</returns>
        public IEnumerable<SoEnchantment> GetRandomEnchantmentList(int count)
        {
            var enchantments = new List<SoEnchantment>();

            var validEnchantments = _unusedEnchantments.Where(e => !e.HasRequirement || _currentEnchantments.ContainsKey(e.Requirement)).ToList();

            for (var i = 0; i < count; i++)
            {
                if (validEnchantments.Count == 0) break;

                var randomIndex = Random.Range(0, validEnchantments.Count);
                var randomEnchantment = validEnchantments[randomIndex];
                
                enchantments.Add(randomEnchantment);
                validEnchantments.Remove(randomEnchantment);
                _unusedEnchantments.Remove(randomEnchantment);
            }

            return enchantments;
        }
        
        public SoEnchantment GetRandomEnchantment(SoEnchantment except = null)
        {
            var validEnchantments = _unusedEnchantments.Where(e => !e.HasRequirement || _currentEnchantments.ContainsKey(e.Requirement)).ToList();

            if (except) validEnchantments.Remove(except);
            
            var randomIndex = Random.Range(0, validEnchantments.Count);
            var randomEnchantment = validEnchantments[randomIndex];

            _unusedEnchantments.Remove(randomEnchantment);

            return randomEnchantment;
        }

        private void RemoveEnchantment(EEnchantmentName enchantmentName)
        {
            // We are only destroying logic prefab, because we still need information
            // that this enchantment was used, thats why it stays in dictionary 
            Destroy(_currentEnchantments[enchantmentName]);
            // This will be likely used only by ui
            OnRemoveEnchantment?.Invoke(enchantmentName);
        }
        
        public bool Has(EEnchantmentName enchantmentName, bool includeUpgrades = false)
        {
            if(!includeUpgrades) return _currentEnchantments.ContainsKey(enchantmentName);
            
            var upgrades = _enchantmentValues.Where(n =>
            {
                var eName = n.ToString()[..^2];
                return _enchantmentMap.ContainsKey(eName);
            }).ToList();
            upgrades.Add(enchantmentName);
            
            return upgrades.Any(VARIABLE => _currentEnchantments.ContainsKey(VARIABLE));
        }

        public int GetStacks(EEnchantmentName enchantmentName)
        {
            if (!_currentEnchantments.TryGetValue(enchantmentName, out var value)) return -1;

            return value is IStackEnchantment enchantment ? enchantment.Stacks : -1;
        }
        
        public bool Ready(EEnchantmentName enchantmentName)
        {
            if (!_currentEnchantments.TryGetValue(enchantmentName, out var value)) return false;

            return value is ICooldownEnchantment cooldownEnchantment && cooldownEnchantment.Ready();
        }
        
    }
}