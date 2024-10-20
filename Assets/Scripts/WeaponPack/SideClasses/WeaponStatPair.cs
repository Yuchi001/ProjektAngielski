using UnityEngine;
using UnityEngine.Serialization;
using WeaponPack.Enums;

namespace WeaponPack
{
    [System.Serializable]
    public class WeaponStatPair
    {
        [SerializeField] protected EWeaponStat statType;
        [SerializeField] protected float statValue;
        [SerializeField] protected bool isPercentage;
        [SerializeField] protected bool canUpgrade = true;

        public float StatValue => statValue;
        public bool IsPercentage => isPercentage;
        public bool CanUpgrade => canUpgrade;
        public EWeaponStat StatType => statType;

        public void SetStatValue(float value)
        {
            statValue = value;
        }
    }
}