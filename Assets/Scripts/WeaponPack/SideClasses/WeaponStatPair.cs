using UnityEngine.Serialization;
using WeaponPack.Enums;

namespace WeaponPack
{
    [System.Serializable]
    public class WeaponStatPair
    {
        public EWeaponStat statType;
        public float statValue;
        public bool isPercentage;
    }
}