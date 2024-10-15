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
    public class PlayerEnchantmentManager : MonoBehaviour
    {
        private readonly Dictionary<EEnchantmentName, EnchantmentBase> _currentEnchantments = new();
        private List<SoEnchantment> _allEnchantments = new();

        public delegate void AddEnchantmentDelegate(SoEnchantment enchantment, EnchantmentBase logic);
        public static event AddEnchantmentDelegate OnAddEnchantment;
        
        private void Awake()
        {
            _allEnchantments = Resources.LoadAll<SoEnchantment>("Enchantments").Select(Instantiate).ToList();
        }
        
        public void AddEnchantment(SoEnchantment enchantmentToAdd)
        {
            var alreadyIn = _currentEnchantments.ContainsKey(enchantmentToAdd.EnchantmentName);
            if (alreadyIn) return;

            var enchantmentLogic = enchantmentToAdd.Setup(transform);
            _currentEnchantments.Add(enchantmentToAdd.EnchantmentName, enchantmentLogic);
            
            OnAddEnchantment?.Invoke(enchantmentToAdd, enchantmentLogic);
        }

        public IEnumerable<SoEnchantment> GetRandomEnchantmentList(int count)
        {
            var enchantments = new List<SoEnchantment>();
            var enchantmentPool = _allEnchantments;

            for (var i = 0; i < count; i++)
            {
                if (enchantmentPool.Count == 0) break;

                var randomIndex = Random.Range(0, enchantmentPool.Count);
                var pickedEnchantment = Instantiate(enchantmentPool[randomIndex]);
                enchantments.Add(pickedEnchantment);
                enchantmentPool.RemoveAt(randomIndex);
            }

            return enchantments.Any(e => _currentEnchantments.ContainsKey(e.EnchantmentName)) ? new List<SoEnchantment>() : enchantments;
        }

        public bool Has(EEnchantmentName enchantmentName)
        {
            return _currentEnchantments.ContainsKey(enchantmentName);
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