using System.Collections.Generic;
using EnchantmentPack;
using Managers;
using Managers.Other;
using UIPack;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack.PlayerEnchantmentPack
{
    public class PlayerEnchantmentUI : UIBase
    {
        [SerializeField] private RectTransform enchantmentContainer;

        private readonly Dictionary<string, EnchantmentItemAccessor> _spawnedEnchantments = new();

        private EnchantmentItemAccessor _enchantmentItemAccessorPrefab;
        
        private void Awake()
        {
            _spawnedEnchantments.Clear();
            
            PlayerEnchantments.OnAddEnchantment += OnEnchantmentAdd;
            PlayerEnchantments.OnRemoveEnchantment += OnRemoveEnchantment;

            _enchantmentItemAccessorPrefab = GameManager.GetPrefab<EnchantmentItemAccessor>(PrefabNames.EnchantmentItemAccessor);
        }

        private void OnDestroy()
        {
            PlayerEnchantments.OnAddEnchantment -= OnEnchantmentAdd;
            PlayerEnchantments.OnRemoveEnchantment -= OnRemoveEnchantment;
        }

        private void OnEnchantmentAdd(EnchantmentLogic logic)
        {
            var displayStrategy = Instantiate(_enchantmentItemAccessorPrefab, enchantmentContainer);
            logic.ApplyDisplayStrategy(displayStrategy);
            _spawnedEnchantments.Add(logic.Enchantment.Name, displayStrategy);
            LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentContainer);
        }

        private void OnRemoveEnchantment(string enchantmentName)
        {
            Destroy(_spawnedEnchantments[enchantmentName].gameObject);
            _spawnedEnchantments.Remove(enchantmentName);
            LayoutRebuilder.ForceRebuildLayoutImmediate(enchantmentContainer);
        }
    }
}