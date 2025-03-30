using ItemPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace ItemPack
{
    [System.Serializable]
    public sealed class StatPair
    {
        [SerializeField] private EItemSelfStatType selfStatType;
        [SerializeField] private float statValue;
        [SerializeField] private bool canUpgrade;
        [SerializeField] private bool useMax;
        [SerializeField] private bool useMin;
        [SerializeField] private float maximum;
        [SerializeField] private float minimum;
        [SerializeField] private float mod;

        public float StatValue => statValue;
        public bool CanUpgrade => canUpgrade;
        public EItemSelfStatType SelfStatType => selfStatType;
        public float Minimum => useMin ? minimum : 0.0f;
        public bool HasMinimum => useMin;
        public float Maximum => useMax ? maximum : 0.0f;
        public bool HasMaximum => useMax;
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
            this.useMax = false;
            this.useMin = false;
            this.minimum = 0;
            this.maximum = 0;
        }
        
        public StatPair(EItemSelfStatType selfStatType, float statValue)
        {
            this.selfStatType = selfStatType;
            this.statValue = statValue;
            this.canUpgrade = false;
        }

        public struct UpgradeProps
        {
            public float min;
            public float max;
            public float mod;
            public bool useMin;
            public bool useMax;
        }
        public void SetUpgradeStats(UpgradeProps props)
        {
            this.canUpgrade = true;
            this.maximum = props.max;
            this.minimum = props.min;
            this.useMax = props.useMax;
            this.useMin = props.useMin;
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
            if (useMax) res = res > maximum ? maximum : res;
            if (useMin) res = res < minimum ? minimum : res;
            return res;
        }
    }
}