using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EnchantmentPack.Enums;
using EnchantmentPack.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace EnchantmentPack
{
    [CreateAssetMenu(fileName = "new Enchantment", menuName = "Custom/Enchantment")]
    public class SoEnchantment : ScriptableObject
    {
        [SerializeField] private Sprite enchantmentSprite;
        [SerializeField, Tooltip("Optional")] private Sprite enchantmentActiveSprite;
        [SerializeField] private GameObject enchantmentLogicPrefab;
        [SerializeField] private EEnchantmentName enchantmentName;
        [SerializeField] private EEnchantmentType enchantmentType;
        [SerializeField] private List<EnchantmentParam> parameters;
        [SerializeField, TextArea(5, 5)] private string description;
        [SerializeField, Tooltip("Optional")] private EEnchantmentName requirement = EEnchantmentName.None;
        
        public Sprite EnchantmentSprite => enchantmentSprite;
        public Sprite EnchantmentActiveSprite => enchantmentActiveSprite;
        public EEnchantmentName EnchantmentName => enchantmentName;
        public EEnchantmentType EEnchantmentType => enchantmentType;
        public IEnumerable<EnchantmentParam> EnchantmentParams => parameters;
        public EEnchantmentName Requirement => requirement;
        public bool HasRequirement => Requirement != EEnchantmentName.None;
        
        public string GetDescription()
        {
            return parameters.Aggregate(description, (current, param) => current.Replace(param.DescriptionKey, GetParamFormat(param)));
        }

        private static string GetParamFormat(EnchantmentParam param)
        {
            var paramInt = (int)param.Value;
            var isFloat = param.Value - paramInt > 0;
            return param.Key switch
            {
                EValueKey.Damage => ((int)(param.Value)).ToString(),
                EValueKey.Value => param.Value.ToString(CultureInfo.InvariantCulture),
                EValueKey.Time => isFloat ? $"{param.Value:0.0}sec" : paramInt.ToString(),
                EValueKey.Rate => isFloat ? $"{(1f / param.Value):0.0}" : paramInt.ToString(),
                EValueKey.Range => isFloat ? $"{param.Value:0.0}" : paramInt.ToString(),
                EValueKey.Percentage => $"{Mathf.CeilToInt(param.Value * 100)}%",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public EnchantmentBase Setup(Transform parent)
        {
            if (enchantmentLogicPrefab == null) return null;
            
            var enchantmentLogicObj = Instantiate(enchantmentLogicPrefab, parent, true);
            var enchantmentLogic = enchantmentLogicObj.GetComponent<EnchantmentBase>();
            enchantmentLogic.Setup(this);
            
            return enchantmentLogic;
        }

        [System.Serializable]
        public struct EnchantmentParam
        {
            [SerializeField] private EValueKey key;
            [SerializeField] private float value;
            [SerializeField] private string descriptionKey;

            public EValueKey Key => key;
            public float Value => value;
            public string DescriptionKey => descriptionKey;
        }
    }
}