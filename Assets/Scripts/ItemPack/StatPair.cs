using ItemPack.Enums;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack
{
    [System.Serializable]
    public class StatPair
    {
        [SerializeField] protected EWeaponStat statType;
        [SerializeField] protected float statValue;
        [SerializeField] protected bool isPercentage;
        [SerializeField] protected bool canUpgrade;
        [SerializeField] private EStatTarget statTarget;

        public float StatValue => statValue;
        public bool IsPercentage => isPercentage;
        public bool CanUpgrade => canUpgrade;
        public EWeaponStat StatType => statType;
        public EStatTarget StatTarget => statTarget;

        public StatPair(StatPair pair)
        {
            this.statType = pair.statType;
            this.statValue = 0;
            this.isPercentage = pair.isPercentage;
            this.canUpgrade = pair.canUpgrade;
        }
        
        public StatPair(StatPair first, float secondValue, bool addition = true)
        {
            this.statType = first.statType;
            this.statValue = addition ? first.statValue + secondValue : secondValue;
            this.isPercentage = first.isPercentage;
            this.canUpgrade = first.canUpgrade;
        }
        
        public StatPair()
        {
            this.statType = 0;
            this.statValue = 0;
            this.isPercentage = false;
            this.canUpgrade = false;
        }
        
        public StatPair(EWeaponStat statType, float statValue, bool isPercentage, bool canUpgrade, EStatTarget statTarget)
        {
            this.statType = statType;
            this.statValue = statValue;
            this.isPercentage = isPercentage;
            this.canUpgrade = canUpgrade;
            this.statTarget = statTarget;
        }

        public void SetStatValue(float value)
        {
            statValue = value;
        }
    }
}