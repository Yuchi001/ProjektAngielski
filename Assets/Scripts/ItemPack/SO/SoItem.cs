using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using PlayerPack.Enums;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new Item", menuName = "Custom/Item")]
    public class SoItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField, TextArea] private string itemDescription;
        [SerializeField] private int itemPrice;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private bool oneTimeSpawnLogic;
        [SerializeField] private ItemLogicBase itemPrefab;
        [SerializeField] private List<EItemTag> itemTags;
        [SerializeField] private EItemType itemType;
        
        [SerializeField] private List<StatPair> startingStats = new();

        public string ItemName => itemName;
        public float ItemPrice => itemPrice;
        public string ItemDescription => itemDescription;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public List<EItemTag> ItemTags => itemTags;
        public EItemType ItemType => itemType;
        public ItemLogicBase ItemPrefab => itemPrefab;
        public Sprite ItemSprite => itemSprite;
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
    }
}