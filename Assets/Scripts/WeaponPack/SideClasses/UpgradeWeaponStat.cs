using System.Collections.Generic;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.SideClasses;

namespace WeaponPack.SideClasses
{
    [System.Serializable]
    public class UpgradeWeaponStat : WeaponStatPair
    {
        [TextArea, SerializeField] private string weaponLevelUpDescription;
        [SerializeField] private int valueLevels;
        [SerializeField] private float valueGrowth;
        
        private const string valueMarker = "<v>";

        public float ValueGrowth => valueGrowth;
        public int ValueLevels => valueLevels;
        public string RawDescription => weaponLevelUpDescription;

        public UpgradeWeaponStat(WeaponStatPair statPair)
        {
            statType = statPair.StatType;
            statValue = statPair.StatValue;
            isPercentage = statPair.IsPercentage;
        }

        public UpgradeWeaponStat(float statValue, EWeaponStat statType, bool isPercentage,
            string weaponLevelUpDescription, int valueLevels, float valueGrowth)
        {
            this.statType = statType;
            this.statValue = statValue;
            this.isPercentage = isPercentage;
            this.weaponLevelUpDescription = weaponLevelUpDescription;
            this.valueGrowth = valueGrowth;
            this.valueLevels = valueLevels;
        }

        public (float value, int level) GetLeveledStatValue()
        {
            var weightSum = 0;
            var level = 0;
            var valueWeightPair = new List<(int weight, float value)>();
            for (var i = 1; i <= valueLevels; i++)
            {
                var weight = 100 / i;
                weightSum += weight;
                valueWeightPair.Add((weight, statValue + valueGrowth * (i - 1)));
            }

            var randomNumber = Random.Range(0, weightSum);
            foreach (var pair in valueWeightPair)
            {
                level++;
                if (randomNumber <= pair.weight) return (pair.value, level);
                randomNumber -= pair.weight;
            }

            return (0,0);
        }
        
        public string GetDescription(float value)
        {
            var absValue = Mathf.Abs(value);
            var statString = isPercentage ? $"{Mathf.FloorToInt(absValue * 100)}%" : $"{absValue}";
            return weaponLevelUpDescription.Replace(valueMarker, statString);
        }

        public static UpgradeWeaponStat GetModifiedStat(UpgradeWeaponStat current, float newVal)
        {
            return new UpgradeWeaponStat(current)
            {
                valueGrowth = current.valueGrowth,
                valueLevels = current.valueLevels,
                statValue = newVal,
                weaponLevelUpDescription = current.weaponLevelUpDescription,
            };
        }
    }
}