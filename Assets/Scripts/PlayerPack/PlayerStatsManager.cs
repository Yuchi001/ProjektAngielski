using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerPack.Enums;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private Dictionary<EPlayerStatType, Stat> _stats = new();

        private void Awake()
        {
            _stats = ((EPlayerStatType[])(System.Enum.GetValues(typeof(EPlayerStatType)))).ToDictionary(v => v, v => new Stat());
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);

            foreach (var stat in PlayerManager.Instance.PickedCharacter.StatDict)
            {
                _stats[stat.Key].ModifyValue(stat.Value);
            }
        }

        public static Dictionary<EPlayerStatType, Stat> EditorDictHelper(Dictionary<EPlayerStatType, float> dict)
        {
            var newDict = new Dictionary<EPlayerStatType, Stat>();
            foreach (var statType in (EPlayerStatType[])System.Enum.GetValues(typeof(EPlayerStatType)))
            {
                var hasValue = dict.TryGetValue(statType, out var current);
                newDict.Add(statType, new Stat(hasValue ? current : 0));
            }
            return newDict;
        }

        public float GetStat(EPlayerStatType statType) 
        {
            return _stats[statType].GetDependentValue(_stats);
        }

        public int GetStatAsInt(EPlayerStatType statType)
        {
            return Mathf.CeilToInt(_stats[statType].GetDependentValue(_stats));
        }

        public float ModifyStat(EPlayerStatType statType, float modifier)
        {
            _stats[statType].ModifyValue(modifier);
            return _stats[statType].Value;
        }

        public class Stat
        {
            public float Value { get; private set; }
            
            private float? limit = null;
            private bool _isInt;
            
            private readonly List<(EPlayerStatType stat, float factor)> _dependencies = new();

            public Stat()
            {
                this.Value = 0;
            }

            public Stat(float value)
            {
                Value = value;
            }

            public Stat AsInt()
            {
                _isInt = true;
                return this;
            }
            
            public float GetDependentValue(Dictionary<EPlayerStatType, Stat> stats)
            {
                var val = Value;
                foreach (var pair in _dependencies)
                {
                    val += stats[pair.stat].Value * pair.factor;

                    if (!limit.HasValue || Value < limit) continue;

                    val = limit.Value;
                    break;
                }

                return _isInt ? Mathf.CeilToInt(val) : val;
            }

            public float ModifyValue(float modifier)
            {
                Value += modifier;
                return Value;
            }

            public Stat SetLimit(float limit)
            {
                this.limit = limit;
                return this;
            }

            public Stat AddDependency(EPlayerStatType stat, float factor)
            {
                _dependencies.Add((stat, factor));
                return this;
            }

            public List<EPlayerStatType> GetDependencies()
            {
                return new List<EPlayerStatType>(_dependencies.Select(d => d.stat));
            }
        }
    }
}