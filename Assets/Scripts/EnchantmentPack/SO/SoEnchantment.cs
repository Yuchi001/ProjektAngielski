using EnchantmentPack.EnchantmentUIStrategy;
using UnityEngine;

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

        public virtual void OnApply(EnchantmentLogic enchantmentLogic) {}
        public virtual void OnUpdate(EnchantmentLogic enchantmentLogic) {}
        public virtual void OnRemove(EnchantmentLogic enchantmentLogic) {}

        public virtual void HandleDisplayStrategy(EnchantmentItemAccessor accessor, EnchantmentLogic logic)
        {
            // IGNORE
        }
    }
}