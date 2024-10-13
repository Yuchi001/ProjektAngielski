using System;
using EnchantmentPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnchantmentPack
{
    [CreateAssetMenu(fileName = "new Enchantment", menuName = "Custom/Enchantment")]
    public class SoEnchantment : ScriptableObject
    {
        [SerializeField] private Sprite enchantmentSprite;
        [SerializeField, Tooltip("Leave empty if don't needed")] private Sprite enchantmentActiveSprite;
        [SerializeField] private GameObject enchantmentLogicPrefab;
        [SerializeField] private EEnchantmentName enchantmentName;
        [SerializeField] private EEnchantmentType enchantmentType;

        public Sprite EnchantmentSprite => enchantmentSprite;
        public Sprite EnchantmentActiveSprite => enchantmentActiveSprite;
        public GameObject EnchantmentLogicPrefab => enchantmentLogicPrefab;
        public EEnchantmentName EnchantmentName => enchantmentName;
        public EEnchantmentType EEnchantmentType => enchantmentType;
    }
}