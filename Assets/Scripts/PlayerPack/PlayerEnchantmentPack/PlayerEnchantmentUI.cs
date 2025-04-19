using System.Collections.Generic;
using EnchantmentPack;
using EnchantmentPack.EnchantmentLogic;
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

        private void OnEnchantmentAdd(EnchantmentLogicBase logic)
        {
            var displayStrategy = Instantiate(_enchantmentItemAccessorPrefab, enchantmentContainer);
            logic.AttachDisplayStrategy(displayStrategy);
            _spawnedEnchantments.Add(logic.GetData.Name, displayStrategy);
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