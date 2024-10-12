using System;
using UnityEngine;

namespace EnchantmentPack
{
    [CreateAssetMenu(fileName = "new Enchantment", menuName = "Custom/Enchantment")]
    public class SoEnchantment : ScriptableObject
    {
        [SerializeField] private string enchantmentName;
        [SerializeField] private Sprite enchantmentSprite;
        [SerializeField, Tooltip("Leave empty if don't needed")] private Sprite enchantmentActiveSprite;
        [SerializeField] private GameObject enchantmentLogicPrefab;
        [SerializeField] private bool hasCooldown;

        public string EnchantmentName => enchantmentName;
        public Sprite EnchantmentSprite => enchantmentSprite;
        public Sprite EnchantmentActiveSprite => enchantmentActiveSprite;
        public GameObject EnchantmentLogicPrefab => enchantmentLogicPrefab;
        public bool HasCooldown => hasCooldown;
    }
}