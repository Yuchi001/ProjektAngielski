using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerPack.Enums;
using PlayerPack.SO;
using UnityEngine;

namespace PlayerPack
{
    //TODO: refactor this monstrosity
    public class PlayerStatsManager : MonoBehaviour
    {
        private Dictionary<EPlayerStatType, Stat> _stats = new();
        
        public void SetCharacter(SoCharacter soCharacter)
        {
            _stats = ((EPlayerStatType[])(System.Enum.GetValues(typeof(EPlayerStatType)))).ToDictionary(v => v, v => new Stat());
            foreach (var stat in PlayerManager.Instance.PickedCharacter.StatDict)
            {
                _stats[stat.Key].ModifyValue(stat.Value);
            }
        }

        public float GetStat(EPlayerStatType statType) 
        {
            return _stats[statType].Value;
        }

        public int GetStatAsInt(EPlayerStatType statType)
        {
            return Mathf.CeilToInt(_stats[statType].Value);
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