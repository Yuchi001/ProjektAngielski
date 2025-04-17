using EnchantmentPack.EnchantmentLogic;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack.SO
{
    public abstract class SoEnchantment : ScriptableObject
    {
        [SerializeField] private string enchantmentName;
        [SerializeField, TextArea(4, 4)] private string enchantmentDescription;
        [SerializeField] private Sprite enchantmentSprite;
        [SerializeField] private int enchantmentLevel;

        public string Name => enchantmentName;
        public Sprite Sprite => enchantmentSprite;
        protected string Description => enchantmentDescription;
        public int Level => enchantmentLevel;

        public abstract string GetDescription();

        public abstract EnchantmentLogicBase GetLogicPrefab();
    }
}