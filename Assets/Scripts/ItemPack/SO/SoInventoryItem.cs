using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new InventoryItem", menuName = "Custom/Item/InventoryItem")]
    public class SoInventoryItem : SoItem
    {
        [SerializeField] private bool oneTimeSpawnLogic;
        [SerializeField] private ItemLogicBase itemPrefab;
        
        [SerializeField] private List<StatPair> startingStats = new();


        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public ItemLogicBase ItemPrefab => itemPrefab;

        public IEnumerable<StatPair> StartingStats
        {
            get
            {
                if (ItemPrefab == null) return null;
                var defaultStats = ItemPrefab.GetUsedStats();
                startingStats.RemoveAll(e => !defaultStats.Contains(e.SelfStatType));
                foreach (var statType in defaultStats)
                {
                    var stat = startingStats.FirstOrDefault(s => s.SelfStatType == statType);
                    if (stat != default) continue;
                    startingStats.Add(new StatPair(statType, 0));
                }

                return startingStats;
            }
        }
        
        public void SetWeaponStartingStats(List<StatPair> stats)
        {
            startingStats = new List<StatPair>(stats);
        }

        public float GetStatValue(EItemSelfStatType statType, int level)
        {
            var startingStat = startingStats.FirstOrDefault(s => s.SelfStatType == statType);

            if (startingStat == null) throw new NullReferenceException($"Stat {statType} not found for weapon: {this.itemName}");
            return startingStat.GetStatValue(level);
        }

        public override bool OnPickUp(params int[] paramArray)
        {
            if (PlayerManager.Instance.PlayerItemManager.IsFull()) return false;
            
            var index = PlayerManager.Instance.PlayerItemManager.AddItem(Instantiate(this), paramArray[0]);
            return index != -1;
        }
    }
}