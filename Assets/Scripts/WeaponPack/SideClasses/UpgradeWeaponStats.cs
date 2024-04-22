using System.Collections.Generic;

namespace WeaponPack
{
    [System.Serializable]
    public class UpgradeWeaponStats
    {
        public string weaponLevelUpDescription;
        public List<WeaponStatPair> levelStats;
    }
}