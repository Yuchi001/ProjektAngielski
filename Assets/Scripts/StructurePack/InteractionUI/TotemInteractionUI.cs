using System;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack.SO;
using TMPro;
using UIPack;
using UnityEngine;
using UnityEngine.UI;

namespace StructurePack.InteractionUI
{
    public class TotemInteractionUI : UIBase, IStructure
    {
        [SerializeField] private RectTransform enchantmentsHolder;

        [SerializeField] private Button rechargeButton;

        private TextMeshProUGUI _rechargeButtonText;
        private SoTotemStructure _totem;
        private int _rechargeCost;

        private readonly List<TotemSlotUI> _slotList = new();
        private StructureBase _structureBase;

        private void Awake()
        {
            _rechargeButtonText = rechargeButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(SoStructure structureData, StructureBase structureBase)
        {
            _slotList.Clear();
            _structureBase = structureBase;
            _totem = (SoTotemStructure)structureData;
            _rechargeCost = _totem.BaseRechargeCost;
            var slotPrefab = GameManager.Instance.GetPrefab<TotemSlotUI>(PrefabNames.TotemSlot);
            var randomEnchantments = PlayerManager.Instance.PlayerEnchantments.GetRandomEnchantmentList(_totem.EnchantmentCount);
            foreach (var enchantment in randomEnchantments)
            {
                var slot = Instantiate(slotPrefab, enchantmentsHolder);
                slot.Setup(enchantment, this);
                _slotList.Add(slot);
            }
            
            enchantmentsHolder.ForceUpdateRectTransforms();
            _rechargeButtonText.text = $"Recharge: {_rechargeCost} lvl's";
        }

        public bool CanUseThisEnchantment(SoEnchantment enchantment)
        {
            return _slotList.All(slot => !slot.HasEnchantment(enchantment));
        }

        public void IncrementRechargeCost()
        {
            _rechargeCost += _totem.RechargeIncrementCost;
            _rechargeButtonText.text = $"Recharge: {_rechargeCost} lvl's";
        }

        private void Update()
        {
            rechargeButton.interactable = _rechargeCost <= PlayerManager.Instance.PlayerExp.StackedLevels;
            if (Input.GetKeyDown(KeyCode.Escape)) OnClose();
        }

        public void Recharge()
        {
            Setup(_totem, _structureBase);
        }

        public void ButtonCloseUI()
        {
            _structureBase.HandleInteraction();
        }

        public override void OnClose()
        {
            _structureBase.HandleCloseUI();
            base.OnClose();
        }
    }
}