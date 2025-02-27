using System;
using System.Linq;
using EnchantmentPack;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StructurePack.InteractionUI
{
    public class TotemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private GameObject purchasedObject;
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private TextMeshProUGUI costField;

        [SerializeField] private Button rollButton;
        [SerializeField] private Button enchantButton;

        private SoEnchantment _enchantment;
        private TotemInteractionUI _totem;

        private void Awake()
        {
            purchasedObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (!_enchantment || !PlayerManager.Instance.PlayerEnchantments.Has(_enchantment.EnchantmentName)) return;
            
            ReRollEnchantment();
        }

        public void Setup(SoEnchantment enchantment, TotemInteractionUI totem)
        {
            _totem = totem;
            image.sprite = enchantment.EnchantmentSprite;
            //nameField.text = enchantment.EnchantmentName.ToString();
            descriptionField.text = enchantment.GetDescription();
            costField.text = enchantment.EnchantmentCost.ToString();

            _enchantment = enchantment;
        }

        public void ReRollEnchantment()
        {
            do
            {
                _enchantment = PlayerManager.Instance.PlayerEnchantments.GetRandomEnchantment(_enchantment);
            } while (!_totem.CanUseThisEnchantment(_enchantment));
            rollButton.interactable = false;
            Setup(_enchantment, _totem);
        }

        public void Enchant()
        {
            enchantButton.interactable = false;
            rollButton.interactable = false;
            PlayerManager.Instance.PlayerEnchantments.AddEnchantment(_enchantment);
            _totem.IncrementRechargeCost();
            purchasedObject.SetActive(true);
        }

        public bool HasEnchantment(SoEnchantment enchantment)
        {
            return _enchantment.EnchantmentName == enchantment.EnchantmentName;
        }

        private void Update()
        {
            enchantButton.interactable = _enchantment != null &&
                                         _enchantment.EnchantmentCost <= PlayerManager.Instance.PlayerExp.StackedLevels;
        }
    }
}