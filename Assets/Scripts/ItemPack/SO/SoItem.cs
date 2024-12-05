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
        [SerializeField] private int itemWeight;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private bool oneTimeSpawnLogic;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private List<EItemTag> itemTags;
        [SerializeField] private EItemType itemType;
        [SerializeField] private List<StatScalePair> itemScalingPower = new();
        
        [SerializeField] private List<StatPair> startingStats = new();
        
        [SerializeField] private TieredStatDict tieredStats = new();

        public string ItemName => itemName;
        public string ItemDescription => itemDescription;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public List<EItemTag> ItemTags => itemTags;
        public EItemType ItemType => itemType;
        public Dictionary<EPlayerStatType, EScalingPower> ItemScalingPower => itemScalingPower.ToDictionary(e => e.StatType, e => e.ScalingPower);
        public int ItemWeight => itemWeight;
        public GameObject ItemPrefab => itemPrefab;
        public Sprite ItemSprite => itemSprite;
        public List<StatPair> StartingStats => startingStats;
        public TieredStatDict TieredStats => tieredStats;
        
        public void InitItemScaling()
        {
            foreach (var statType in (EPlayerStatType[])System.Enum.GetValues(typeof(EPlayerStatType)))
            {
                if (itemScalingPower.FirstOrDefault(i => i.StatType == statType) != default) continue;
                itemScalingPower.Add(new StatScalePair(statType, EScalingPower.NONE));
            }
        }

        public void SetWeaponTieredStats(Dictionary<int, List<StatPair>> stats)
        {
            tieredStats.Set(stats);
        }
        
        public void SetWeaponStartingStats(List<StatPair> stats)
        {
            startingStats = new List<StatPair>(stats);
        }

        public StatPair GetStat(EItemSelfStatType statTypeType, int level)
        {
            var startingStat = StartingStats.FirstOrDefault(s => s.SelfStatType == statTypeType);
            if (startingStat == default) return null;

            if (level == 0) return new StatPair(startingStat);

            var contains = TieredStats.Dict.TryGetValue(level, out var stats);
            if (!contains) return null;

            var modifier = stats.FirstOrDefault(s => s.SelfStatType == statTypeType);
            if (modifier == default) return null;

            return new StatPair(startingStat, modifier.StatValue);
        }

        public void SetScalingPower(List<StatScalePair> statScalePairs)
        {
            itemScalingPower = statScalePairs;
        }
        

        [System.Serializable]
        public class TieredStatPair
        {
            [SerializeField] private int _level;
            [SerializeField] private List<StatPair> _stats;

            public int Level => _level;
            public List<StatPair> Stats => _stats;

            public TieredStatPair(KeyValuePair<int, List<StatPair>> pair)
            {
                _level = pair.Key;
                _stats = pair.Value;
            }
            
            public TieredStatPair()
            {
                _level = 0;
                _stats = new List<StatPair>();
            }
        }

        [System.Serializable]
        public class TieredStatDict
        {
            [SerializeField] private List<TieredStatPair> _pairs = new();

            public Dictionary<int, List<StatPair>> Dict => _pairs.ToDictionary(p => p.Level, p => p.Stats);

            public void Set(Dictionary<int, List<StatPair>> dict)
            {
                _pairs = dict.Select(e => new TieredStatPair(e)).ToList();
            }
        }

        [System.Serializable]
        public class StatScalePair
        {
            [SerializeField] private EPlayerStatType _statType;
            [SerializeField] private EScalingPower _scalingPower;

            public EPlayerStatType StatType => _statType;
            public EScalingPower ScalingPower => _scalingPower;

            public StatScalePair(EPlayerStatType statType, EScalingPower scalingPower)
            {
                _statType = statType;
                _scalingPower = scalingPower;
            }
        }
    }
}