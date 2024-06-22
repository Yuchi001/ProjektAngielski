using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        
        [SerializeField] private List<UpgradeWeaponStat> weaponUpgradeStats = new();
        public List<UpgradeWeaponStat> WeaponUpgradeStats => weaponUpgradeStats;

        public string WeaponName => weaponName;
        public string WeaponDescription => weaponDescription;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public Color WeaponColor => weaponColor;
        public GameObject WeaponLogicPrefab => weaponLogicPrefab;
        public Sprite WeaponSprite => weaponSprite;
        public List<WeaponStatPair> WeaponStartingStats => weaponStartingStats;
        
        
        private (string description, List<WeaponStatPair> stats, int level)? nextLevelStats = null;

        private void GenerateNextLevelStats()
        {
            var stats = new List<WeaponStatPair>();
            var description = "";
            var level = 0;
            
            var maxLevelCount = maxLevelPrize < weaponUpgradeStats.Count ? maxLevelPrize : weaponUpgradeStats.Count - 1;
            var levelCount = Random.Range(1, maxLevelCount);
            var unusedStats = weaponUpgradeStats.Select(s => s.StatType).ToList();
            for (var i = 0; i < levelCount; i++)
            {
                level++;
                var randomIndex = Random.Range(0, unusedStats.Count());
                var statType = unusedStats[randomIndex];
                var stat = GetStatPair(statType);
                level += stat.level;
                stats.Add(stat.stat);
                description += $"{stat.description}. ";
                unusedStats.Remove(statType);
            }

            nextLevelStats = (description, stats, level);
        }

        private (string description, WeaponStatPair stat, int level) GetStatPair(EWeaponStat statType)
        {
            var currentStat = weaponUpgradeStats.FirstOrDefault(w => w.StatType == statType);
            if (currentStat == default) return ("", null, 0);

            var result = currentStat.GetLeveledStatValue();
            var description = currentStat.GetDescription(result.value);
            
            return (description, UpgradeWeaponStat.GetModifiedStat(currentStat, result.value), result.level);
        }

        public string GetNextLevelDescription()
        {
            if(nextLevelStats == null) GenerateNextLevelStats();
            return nextLevelStats?.description ?? "";
        }

        public int GetNextLevelEnchantmentLevel()
        {
            if(nextLevelStats == null) GenerateNextLevelStats();

            return nextLevelStats?.level ?? 0;
        }

        public IEnumerable<WeaponStatPair> GetNextLevelStats()
        {
            if(nextLevelStats == null) GenerateNextLevelStats();
            
            var stats = new List<WeaponStatPair>(nextLevelStats?.stats ?? new List<WeaponStatPair>());
            nextLevelStats = null;
            return stats;
        }

        public void SetWeaponUpgradeStats(IEnumerable<UpgradeWeaponStat> upgradeWeaponStats)
        {
            weaponUpgradeStats = new List<UpgradeWeaponStat>(upgradeWeaponStats);
        }
    }
}