using ItemPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponPack.Enums;

namespace ItemPack
{
    [System.Serializable]
    public class StatPair
    {
        [SerializeField] protected EItemSelfStatType selfStatType;
        [SerializeField] protected float statValue;
        [SerializeField] protected bool isPercentage;
        [SerializeField] protected bool canUpgrade;

        public float StatValue => statValue;
        public bool IsPercentage => isPercentage;
        public bool CanUpgrade => canUpgrade;
        public EItemSelfStatType SelfStatType => selfStatType;

        public StatPair(StatPair pair)
        {
            this.selfStatType = pair.selfStatType;
            this.statValue = 0;
            this.isPercentage = pair.isPercentage;
            this.canUpgrade = pair.canUpgrade;
        }
        
        public StatPair(StatPair first, float secondValue, bool addition = true)
        {
            this.selfStatType = first.selfStatType;
            this.statValue = addition ? first.statValue + secondValue : secondValue;
            this.isPercentage = first.isPercentage;
            this.canUpgrade = first.canUpgrade;
        }
        
        public StatPair()
        {
            this.selfStatType = 0;
            this.statValue = 0;
            this.isPercentage = false;
            this.canUpgrade = false;
        }
        
        public StatPair(EItemSelfStatType selfStatType, float statValue, bool isPercentage, bool canUpgrade)
        {
            this.selfStatType = selfStatType;
            this.statValue = statValue;
            this.isPercentage = isPercentage;
            this.canUpgrade = canUpgrade;
        }

        public void SetStatValue(float value)
        {
            statValue = value;
        }
    }
}