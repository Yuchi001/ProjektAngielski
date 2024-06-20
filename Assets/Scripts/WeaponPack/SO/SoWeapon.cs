using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
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
        [SerializeField] private Color weaponColor;
        [SerializeField] private bool oneTimeSpawnLogic = false;
        [SerializeField] private GameObject weaponLogicPrefab;
        
        [SerializeField] private int maxLevelPrize = 3;

        [SerializeField] private List<WeaponStatPair> weaponStartingStats;
        
        private List<UpgradeWeaponStat> weaponUpgradeStats = new();
        public List<UpgradeWeaponStat> WeaponUpgradeStats => weaponUpgradeStats;

        public string WeaponName => weaponName;
        public string WeaponDescription => weaponDescription;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public Color WeaponColor => weaponColor;
        public GameObject WeaponLogicPrefab => weaponLogicPrefab;
        public Sprite WeaponSprite => weaponSprite;
        public List<WeaponStatPair> WeaponStartingStats => weaponStartingStats;
        
        
        private (string description, List<WeaponStatPair> stats)? nextLevelStats = null;

        private void GenerateNextLevelStats()
        {
            var stats = new List<WeaponStatPair>();
            var description = "";
            
            var maxLevelCount = maxLevelPrize < weaponUpgradeStats.Count ? maxLevelPrize : weaponUpgradeStats.Count - 1;
            var levelCount = Random.Range(0, maxLevelCount);
            var unusedStats = weaponUpgradeStats.Select(s => s.StatType).ToList();
            for (var i = 0; i < levelCount; i++)
            {
                var randomIndex = Random.Range(0, unusedStats.Count());
                var statType = unusedStats[randomIndex];
                var stat = GetStatPair(statType);
                stats.Add(stat.stat);
                description += stat.description;
                unusedStats.Remove(statType);
            }

            nextLevelStats = (description, stats);
        }

        private (string description, WeaponStatPair stat) GetStatPair(EWeaponStat statType)
        {
            var currentStat = weaponUpgradeStats.FirstOrDefault(w => w.StatType == statType);
            if (currentStat == default) return ("", null);

            var value = currentStat.GetLeveledStatValue();
            var description = currentStat.GetDescription(value);
            
            return (description, UpgradeWeaponStat.GetModifiedStat(currentStat, value));
        }

        public string GetNextLevelDescription()
        {
            if(nextLevelStats == null) GenerateNextLevelStats();
            return nextLevelStats?.description ?? "";
        }

        public IEnumerable<WeaponStatPair> GetNextLevelStats()
        {
            if(nextLevelStats == null) GenerateNextLevelStats();
            
            var stats = new List<WeaponStatPair>(nextLevelStats?.stats ?? new List<WeaponStatPair>());
            nextLevelStats = null;
            return stats;
        }

        public void SetWeaponUpgradeStats(List<UpgradeWeaponStat> upgradeWeaponStats)
        {
            weaponUpgradeStats = upgradeWeaponStats;
        }
    }
}