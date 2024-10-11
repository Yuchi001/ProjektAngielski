using System;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerEnchantmentManager : MonoBehaviour
    {
        private readonly List<EnchantmentBase> _currentEnchantments = new();
        private List<SoEnchantment> _allEnchantments = new();

        private readonly List<string> _usedEnchantments = new();

        public delegate void EnchantmentAddDelegate(EnchantmentBase enchantment);
        public static event EnchantmentAddDelegate OnEnchantmentAdd;
        

        private void Awake()
        {
            _allEnchantments = Resources.LoadAll<SoEnchantment>("Enchantments").Select(Instantiate).ToList();
        }

        public void AddEnchantment(SoEnchantment enchantmentToAdd)
        {
            var alreadyIn = _currentEnchantments.Any(e => e.Is(enchantmentToAdd));
            if (alreadyIn) return;

            _usedEnchantments.Add(enchantmentToAdd.EnchantmentName);
            var enchantmentLogicObj = Instantiate(enchantmentToAdd.EnchantmentLogicPrefab, transform, true);
            var enchantmentLogic = enchantmentLogicObj.GetComponent<EnchantmentBase>();
            _currentEnchantments.Add(enchantmentLogic);
            
            OnEnchantmentAdd?.Invoke(enchantmentLogic);
        }

        public IEnumerable<SoEnchantment> GetRandomEnchantmentList(int count)
        {
            var enchantments = new List<SoEnchantment>();
            var enchantmentPool =
                new List<SoEnchantment>(_allEnchantments.Where(e => !_usedEnchantments.Contains(e.EnchantmentName)));

            for (var i = 0; i < count; i++)
            {
                if (enchantmentPool.Count == 0) break;

                var randomIndex = Random.Range(0, enchantmentPool.Count);
                var pickedEnchantment = Instantiate(enchantmentPool[randomIndex]);
                enchantments.Add(pickedEnchantment);
                enchantmentPool.RemoveAt(randomIndex);
            }

            return enchantments;
        }
    }
}