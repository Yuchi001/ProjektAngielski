using EnchantmentPack;
using Managers;
using Managers.Other;
using PlayerPack;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    public class SoTotemStructure : SoStructure
    {
        [SerializeField] private int baseCost;
        [SerializeField] private int stageBaseMultiplier;
        [SerializeField] private int transactionMultiplier;

        private static PlayerSoulManager PlayerSoulManager => PlayerManager.Instance.PlayerSoulManager;
        private static PlayerEnchantments PlayerEnchantments => PlayerManager.Instance.PlayerEnchantments;
        
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            var data = structureBase.GetData<TotemData>();
            var price = data.GetCurrentPrice();
            if (PlayerSoulManager.GetCurrentSoulCount() < price) return false;

            PlayerSoulManager.AddSouls(-price);
            data.MultiplyCurrentPrice(transactionMultiplier);
            
            var addedEnchantment = PlayerEnchantments.AddRandomEnchantment();
            data.DisplayEnchantment(addedEnchantment);
            
            return true;
        }

        public override string GetInteractionMessage(StructureBase structureBase)
        {
            var data = structureBase.GetData<TotemData>();
            if (!data.IsInitialized()) data.Init(baseCost + GameManager.Instance.StageCount * stageBaseMultiplier);
            return base.GetInteractionMessage(structureBase).Replace("$x$", data.GetCurrentPrice().ToString());
        }
        
        private class TotemData
        {
            private int _currentPrice = 0;
            private bool _initialized = false;

            private IOpenStrategy _enchantmentDisplayOpenStrat;
            private ICloseStrategy _enchantmentDisplayCloseStrat;

            private string UI_KEY = "ENCHANTMENT_DISPLAY_KEY";

            public void Init(int currentPrice)
            {
                _currentPrice = currentPrice;
                _initialized = true;

                var prefab = GameManager.Instance.GetPrefab<EnchantmentDisplayUI>(PrefabNames.EnchantmentDisplayUI);
                _enchantmentDisplayOpenStrat = new DefaultOpenStrategy(prefab);
                _enchantmentDisplayCloseStrat = new DestroyCloseStrategy(UI_KEY, 0);
            }

            public void DisplayEnchantment(SoEnchantment enchantment)
            {
                var ui = UIManager.OpenUI<EnchantmentDisplayUI>(UI_KEY, _enchantmentDisplayOpenStrat,
                    _enchantmentDisplayCloseStrat);
                ui.SetData(enchantment);
            }

            public bool IsInitialized() => _initialized;

            public void MultiplyCurrentPrice(int amount)
            {
                _currentPrice *= amount;
            }

            public int GetCurrentPrice() => _currentPrice;
        }
    }
}