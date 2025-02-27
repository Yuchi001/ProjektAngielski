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
        [SerializeField] protected bool canUpgrade;
        [SerializeField] protected float? maximum;
        [SerializeField] protected float? minimum;
        [SerializeField] protected float mod;

        public float StatValue => statValue;
        public bool CanUpgrade => canUpgrade;
        public EItemSelfStatType SelfStatType => selfStatType;
        public float Minimum => minimum ?? 0.0f;
        public float Maximum => maximum ?? 0.0f;
        public float Mod => mod;

        public StatPair(StatPair pair)
        {
            this.selfStatType = pair.selfStatType;
            this.statValue = 0;
            this.canUpgrade = pair.canUpgrade;
            this.maximum = pair.maximum;
            this.minimum = pair.minimum;
        }
        
        public StatPair()
        {
            this.selfStatType = 0;
            this.statValue = 0;
            this.canUpgrade = false;
            this.minimum = null;
            this.maximum = null;
        }
        
        public StatPair(EItemSelfStatType selfStatType, float statValue)
        {
            this.selfStatType = selfStatType;
            this.statValue = statValue;
            this.canUpgrade = false;
        }

        public struct UpgradeProps
        {
            public float? min;
            public float? max;
            public float mod;
        }
        public void SetUpgradeStats(UpgradeProps props)
        {
            this.canUpgrade = true;
            this.maximum = props.max;
            this.minimum = props.min;
            this.mod = props.mod;
        }

        public void SetStatValue(float value)
        {
            statValue = value;
        }

        public float GetStatValue(int level)
        {
            if (!canUpgrade) return statValue;

            var res = statValue + (mod * (level - 1));
            res = res > maximum ? maximum.Value : res;
            res = res < minimum ? minimum.Value : res;
            return res;
        }
    }
}