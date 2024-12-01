using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponPack.Enums;
using WeaponPack.SideClasses;

namespace WeaponPack.SO
{
    [CreateAssetMenu(fileName = "new Weapon", menuName = "Custom/Weapon")]
    public class SoWeapon : ScriptableObject
    {
        [SerializeField] private string weaponName;
        [SerializeField, TextArea] private string weaponDescription;
        [SerializeField] private Sprite weaponSprite;
        [SerializeField] private bool oneTimeSpawnLogic;
        [SerializeField] private GameObject weaponLogicPrefab;
        [SerializeField] private EWeaponCategory weaponCategory;
        
        [SerializeField] private List<WeaponStatPair> weaponStartingStats = new();
        
        [SerializeField] private WeaponTieredStatDict weaponTieredStats = new();

        public string WeaponName => weaponName;
        public string WeaponDescription => weaponDescription;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public EWeaponCategory WeaponCategory => weaponCategory;
        public GameObject WeaponLogicPrefab => weaponLogicPrefab;
        public Sprite WeaponSprite => weaponSprite;
        public List<WeaponStatPair> WeaponStartingStats => weaponStartingStats;
        public WeaponTieredStatDict WeaponTieredStats => weaponTieredStats;

        public void SetWeaponTieredStats(Dictionary<ETierType, List<WeaponStatPair>> stats)
        {
            weaponTieredStats.Set(stats);
        }
        
        public void SetWeaponStartingStats(List<WeaponStatPair> stats)
        {
            weaponStartingStats = new List<WeaponStatPair>(stats);
        }

        public WeaponStatPair GetStat(EWeaponStat statType, ETierType tierType)
        {
            var startingStat = WeaponStartingStats.FirstOrDefault(s => s.StatType == statType);
            if (startingStat == default) return null;

            if (tierType == ETierType.Common) return new WeaponStatPair(startingStat);

            var contains = WeaponTieredStats.Dict.TryGetValue(tierType, out var stats);
            if (!contains) return null;

            var modifier = stats.FirstOrDefault(s => s.StatType == statType);
            if (modifier == default) return null;

            return new WeaponStatPair(startingStat, modifier.StatValue);
        }
        

        [System.Serializable]
        public class WeaponTieredStarPair
        {
            [SerializeField] private ETierType _type;
            [SerializeField] private List<WeaponStatPair> _stats;

            public ETierType Type => _type;
            public List<WeaponStatPair> Stats => _stats;

            public WeaponTieredStarPair(KeyValuePair<ETierType, List<WeaponStatPair>> pair)
            {
                _type = pair.Key;
                _stats = pair.Value;
            }
            
            public WeaponTieredStarPair()
            {
                _type = ETierType.Common;
                _stats = new List<WeaponStatPair>();
            }
        }

        [System.Serializable]
        public class WeaponTieredStatDict
        {
            [SerializeField] private List<WeaponTieredStarPair> _pairs = new();

            public Dictionary<ETierType, List<WeaponStatPair>> Dict => _pairs.ToDictionary(p => p.Type, p => p.Stats);

            public void Set(Dictionary<ETierType, List<WeaponStatPair>> dict)
            {
                _pairs = dict.Select(e => new WeaponTieredStarPair(e)).ToList();
            }
        }
    }
}