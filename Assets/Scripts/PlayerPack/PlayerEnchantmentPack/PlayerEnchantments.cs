using System;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack;
using EnchantmentPack.SO;
using EnchantmentPack.SO.Default;
using Managers;
using Managers.Other;
using UnityEngine;

namespace PlayerPack.PlayerEnchantmentPack
{
    public class PlayerEnchantments : MonoBehaviour
    {
        private static PlayerEnchantments Instance => PlayerManager.PlayerEnchantments;

        private readonly Dictionary<string, EnchantmentLogic> _spawnedEnchantments = new();
        private Dictionary<string, Stack<SoEnchantment>> _enchantmentPool;

        public delegate void AddEnchantmentDelegate(EnchantmentLogic logic);
        public static AddEnchantmentDelegate OnAddEnchantment;
        
        public delegate void RemoveEnchantmentDelegate(string enchantmentName);
        public static RemoveEnchantmentDelegate OnRemoveEnchantment;

        private EnchantmentLogic _enchantmentLogicPrefab;

        private void Awake()
        {
            _enchantmentPool = new Dictionary<string, Stack<SoEnchantment>>();
            _enchantmentLogicPrefab = GameManager.GetPrefab<EnchantmentLogic>(PrefabNames.EnchantmentLogic);

            var allEnchantments = Resources.LoadAll<SoEnchantment>("Enchantments").ToList();

            var grouped = allEnchantments
                .GroupBy(e => e.Name);

            foreach (var group in grouped)
            {
                var enchantmentName = group.Key;
                var enchantments = group.OrderBy(e => e.Level).ToList();

                for (var i = 0; i < enchantments.Count; i++)
                {
                    if (enchantments[i].Level != i + 1)
                    {
                        throw new Exception($"Enchantment group '{enchantmentName}' has missing or invalid levels. Expected level {i + 1}, but got {enchantments[i].Level}.");
                    }
                }

                var stack = new Stack<SoEnchantment>();
                for (var i = enchantments.Count - 1; i >= 0; i--)
                {
                    stack.Push(enchantments[i]);
                }

                _enchantmentPool.Add(enchantmentName, stack);
            }

            var enchant = GetRandomEnchantment();
            TryAddEnchantment(enchant.Name);
        }

        public static SoEnchantment GetRandomEnchantment()
        {
            if (Instance._enchantmentPool == null || Instance._enchantmentPool.Count == 0) throw new Exception("No enchantments available in the pool.");

            var keys = Instance._enchantmentPool.Keys.ToList();
            var randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];

            var stack = Instance._enchantmentPool[randomKey];
    
            if (stack.Count == 0) throw new Exception($"Enchantment stack for '{randomKey}' is empty.");

            return stack.Peek();
        }

        public static bool TryAddEnchantment(string enchantmentName)
        {
            if (!Instance._enchantmentPool.TryGetValue(enchantmentName, out var stack)) return false;

            if (stack.Count == 0)
            {
                Instance._enchantmentPool.Remove(enchantmentName);
                return false;
            }

            var top = stack.Pop();
            var logic = Instantiate(Instance._enchantmentLogicPrefab);

            var success = Instance._spawnedEnchantments.TryAdd(top.Name, logic);
            if (!success)
            {
                OnRemoveEnchantment?.Invoke(top.Name); // invoke before actually deleting enchant so logic script will not be null
                Instance._spawnedEnchantments[top.Name].Remove();
                Instance._spawnedEnchantments[top.Name] = logic;
            }

            logic.Add(top); // apply before event, because we need initialization before subscribers can access enchantment
            OnAddEnchantment?.Invoke(logic);
            
            if (stack.Count == 0) Instance._enchantmentPool.Remove(enchantmentName);

            return true;
        }
    }
}