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

        public float StatValue => statValue;
        public bool IsPercentage => isPercentage;
        public EWeaponStat StatType => statType;

        public void SetStatValue(float value)
        {
            statValue = value;
        }
    }
}