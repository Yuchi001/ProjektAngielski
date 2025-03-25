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
        public List<StatPair> StartingStats => startingStats;
        
        public void SetWeaponStartingStats(List<StatPair> stats)
        {
            startingStats = new List<StatPair>(stats);
        }

        public float? GetStatValue(EItemSelfStatType statTypeType, int level)
        {
            var startingStat = StartingStats.FirstOrDefault(s => s.SelfStatType == statTypeType);

            return startingStat?.GetStatValue(level);
        }

        public override bool OnPickUp(params int[] paramArray)
        {
            if (PlayerManager.Instance.PlayerItemManager.IsFull()) return false;
            
            var index = PlayerManager.Instance.PlayerItemManager.AddItem(Instantiate(this), paramArray[0]);
            return index != -1;
        }
    }
}