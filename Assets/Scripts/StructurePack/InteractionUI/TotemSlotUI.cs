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
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private TextMeshProUGUI costField;

        [SerializeField] private Button rollButton;
        [SerializeField] private Button enchantButton;

        private SoEnchantment _enchantment;
        
        public void Setup(SoEnchantment enchantment)
        {
            image.sprite = enchantment.EnchantmentSprite;
            nameField.text = enchantment.EnchantmentName.ToString();
            descriptionField.text = enchantment.GetDescription();
            costField.text = enchantment.EnchantmentCost.ToString();

            _enchantment = enchantment;
        }

        public void ReRollEnchantment()
        {
            _enchantment = PlayerManager.Instance.PlayerEnchantments.GetRandomEnchantmentList(0).ElementAt(0);
            rollButton.interactable = false;
        }

        public void Enchant()
        {
            enchantButton.interactable = false;
            rollButton.interactable = false;
            PlayerManager.Instance.PlayerEnchantments.AddEnchantment(_enchantment);
        }

        private void Update()
        {
            enchantButton.interactable = _enchantment != null &&
                                         _enchantment.EnchantmentCost <= PlayerManager.Instance.PlayerExp.StackedLevels;
        }
    }
}