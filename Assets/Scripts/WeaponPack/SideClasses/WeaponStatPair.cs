using UnityEngine;
using WeaponPack.Enums;

namespace WeaponPack.SideClasses
{
    [System.Serializable]
    public class WeaponStatPair
    {
        [SerializeField] protected EWeaponStat statType;
        [SerializeField] protected float statValue;
        [SerializeField] protected bool isPercentage;
        [SerializeField] protected bool canUpgrade;

        public float StatValue => statValue;
        public bool IsPercentage => isPercentage;
        public bool CanUpgrade => canUpgrade;
        public EWeaponStat StatType => statType;

        public WeaponStatPair(EWeaponStat statType, float statValue, bool isPercentage)
        {
            this.statType = statType;
            this.statValue = statValue;
            this.isPercentage = isPercentage;
        }
        
        public WeaponStatPair(WeaponStatPair pair)
        {
            this.statType = pair.statType;
            this.statValue = 0;
            this.isPercentage = pair.isPercentage;
            this.canUpgrade = pair.canUpgrade;
        }
        
        public WeaponStatPair(WeaponStatPair first, float secondValue)
        {
            this.statType = first.statType;
            this.statValue = first.statValue + secondValue;
            this.isPercentage = first.isPercentage;
            this.canUpgrade = first.canUpgrade;
        }
        
        public WeaponStatPair()
        {
            this.statType = 0;
            this.statValue = 0;
            this.isPercentage = false;
            this.canUpgrade = false;
        }
        
        public WeaponStatPair(EWeaponStat statType, float statValue, bool isPercentage, bool canUpgrade)
        {
            this.statType = statType;
            this.statValue = statValue;
            this.isPercentage = isPercentage;
            this.canUpgrade = canUpgrade;
        }

        public void SetStatValue(float value)
        {
            statValue = value;
        }
    }
}