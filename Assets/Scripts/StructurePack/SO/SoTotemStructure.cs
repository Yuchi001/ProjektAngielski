﻿using System;
using EnchantmentPack;
using EnchantmentPack.SO;
using Managers;
using Managers.Other;
using PlayerPack;
using PlayerPack.PlayerEnchantmentPack;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Totem Structure", menuName = "Custom/Structure/Totem")]
    public class SoTotemStructure : SoStructure
    {
        [SerializeField] private int baseCost;
        [SerializeField] private int stageBaseMultiplier;
        [SerializeField] private int transactionMultiplier;

        private static PlayerEnchantments PlayerEnchantments => PlayerManager.PlayerEnchantments;
        
        public override bool OnInteract(StructureBase structureBase)
        {
            var data = structureBase.GetData<TotemData>();
            var price = data.GetCurrentPrice();
            if (!PlayerCollectibleManager.HasCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SOUL, price)) return false;

            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SOUL, -price);
            data.MultiplyCurrentPrice(transactionMultiplier);
            
            // TODO: dźwięk dostawania enchantmentu
            
            var enchantmentToAdd = PlayerEnchantments.GetRandomEnchantment();
            var success = PlayerEnchantments.TryAddEnchantment(enchantmentToAdd.Name);
            if (!success) throw new ArgumentOutOfRangeException($"Enchantment: {enchantmentToAdd.Name} couldn't be added to player inv!");
            data.DisplayEnchantment(enchantmentToAdd);
            
            return true;
        }

        public override string GetInteractionMessage(StructureBase structureBase)
        {
            var data = structureBase.GetData<TotemData>();
            if (!data.IsInitialized()) data.Init().InitPrice(baseCost + GameManager.StageCount * stageBaseMultiplier);
            return base.GetInteractionMessage(structureBase).Replace("$x$", data.GetCurrentPrice().ToString());
        }
        
        private class TotemData : BaseStructureData<TotemData>
        {
            private int _currentPrice = 0;

            private IOpenStrategy _enchantmentDisplayOpenStrat;
            private ICloseStrategy _enchantmentDisplayCloseStrat;

            private string UI_KEY = "ENCHANTMENT_DISPLAY_KEY";

            public override TotemData Init()
            {
                var prefab = GameManager.GetPrefab<EnchantmentDisplayUI>(PrefabNames.EnchantmentDisplayUI);
                _enchantmentDisplayOpenStrat = new DefaultOpenStrategy(prefab);
                _enchantmentDisplayCloseStrat = new DestroyCloseStrategy(UI_KEY);
                return base.Init();
            }

            public void InitPrice(int currentPrice)
            {
                _currentPrice = currentPrice;
            }

            public void DisplayEnchantment(SoEnchantment enchantment)
            {
                var ui = UIManager.OpenUI<EnchantmentDisplayUI>(UI_KEY, _enchantmentDisplayOpenStrat,
                    _enchantmentDisplayCloseStrat);
                ui.SetData(enchantment);
            }

            public void MultiplyCurrentPrice(int amount)
            {
                _currentPrice *= amount;
            }

            public int GetCurrentPrice() => _currentPrice;
        }
    }
}