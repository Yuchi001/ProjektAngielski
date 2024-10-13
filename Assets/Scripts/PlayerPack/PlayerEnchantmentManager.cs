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
        private readonly List<EnchantmentBase> _currentEnchantments = new();
        private List<SoEnchantment> _allEnchantments = new();
        
        public delegate void EnchantmentAddDelegate(EnchantmentBase enchantment);
        public static event EnchantmentAddDelegate OnEnchantmentAdd;
        
        private void Awake()
        {
            _allEnchantments = Resources.LoadAll<SoEnchantment>("Enchantments").Select(Instantiate).ToList();
        }
        
        public EnchantmentBase AddEnchantment(SoEnchantment enchantmentToAdd)
        {
            var alreadyIn = _currentEnchantments.Any(e => e.Is(enchantmentToAdd));
            if (alreadyIn) return null;

            var enchantmentLogicObj = Instantiate(enchantmentToAdd.EnchantmentLogicPrefab, transform, true);
            var enchantmentLogic = enchantmentLogicObj.GetComponent<EnchantmentBase>();
            enchantmentLogic.Setup(enchantmentToAdd);
            _currentEnchantments.Add(enchantmentLogic);
            
            OnEnchantmentAdd?.Invoke(enchantmentLogic);

            return enchantmentLogic;
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

            return enchantments;
        }

        public bool Has(EEnchantmentName enchantmentName)
        {
            return _currentEnchantments.Any(e => e.Get().EnchantmentName == enchantmentName);
        }

        public int GetStacks(EEnchantmentName enchantmentName)
        {
            var enchantmentBase = _currentEnchantments.FirstOrDefault(e => e.Get().EnchantmentName == enchantmentName);

            return enchantmentBase == default ? 0 : ((IStackEnchantment)enchantmentBase).Stacks;
        }
    }
}