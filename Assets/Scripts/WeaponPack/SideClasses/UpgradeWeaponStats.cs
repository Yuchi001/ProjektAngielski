using System.Collections.Generic;
using UnityEngine;

namespace WeaponPack
{
    [System.Serializable]
    public class UpgradeWeaponStats
    {
        [TextArea] public string weaponLevelUpDescription;
        public List<WeaponStatPair> levelStats;
    }
}