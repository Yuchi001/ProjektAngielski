using System;
using UnityEngine;

namespace EnchantmentPack
{
    [CreateAssetMenu(fileName = "new Enchantment", menuName = "Custom/Enchantment")]
    public class SoEnchantment : ScriptableObject
    {
        [SerializeField] private string enchantmentName;
        [SerializeField] private Sprite enchantmentSprite;
        [SerializeField] private GameObject enchantmentLogicPrefab;

        public string EnchantmentName => enchantmentName;
        public Sprite EnchantmentSprite => enchantmentSprite;
        public GameObject EnchantmentLogicPrefab => enchantmentLogicPrefab;
    }
}