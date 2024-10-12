using System;
using System.Collections;
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
            PlayerManager.OnPlayerReady += OnPlayerReady;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerReady -= OnPlayerReady;
        }

        private void OnPlayerReady()
        {
            // TODO: Tutaj na razie na sztywno dodaje heal, ale powinnismy dodawac tutaj liste brana z GameManager
            var startingEnchantments = _allEnchantments.Where(e => e.EnchantmentName == "Heal").ToList();

            foreach (var enchantment in startingEnchantments)
            {
                AddEnchantment(enchantment);
            }
        }

        public void AddEnchantment(SoEnchantment enchantmentToAdd)
        {
            var alreadyIn = _currentEnchantments.Any(e => e.Is(enchantmentToAdd));
            if (alreadyIn) return;

            _usedEnchantments.Add(enchantmentToAdd.EnchantmentName);
            var enchantmentLogicObj = Instantiate(enchantmentToAdd.EnchantmentLogicPrefab, transform, true);
            var enchantmentLogic = enchantmentLogicObj.GetComponent<EnchantmentBase>();
            enchantmentLogic.Setup(enchantmentToAdd);
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