using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
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
            
            AddDependencies(_stats);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => PlayerManager.Instance != null);

            foreach (var stat in PlayerManager.Instance.PickedCharacter.StatDict)
            {
                _stats[stat.Key].ModifyValue(stat.Value);
            }
        }

        private static void AddDependencies(Dictionary<EPlayerStatType, Stat> stats)
        {
            //DEXTERITY
            stats[EPlayerStatType.MovementSpeed].AddDependency(EPlayerStatType.Dexterity, StaticOptions.MOVEMENT_SPEED_PER_DEXTERITY_POINT);
            stats[EPlayerStatType.DashStacks].AsInt().AddDependency(EPlayerStatType.Dexterity, StaticOptions.DASH_STACK_PER_DEXTERITY_POINT);
            
            //STRENGTH
            stats[EPlayerStatType.Capacity].AsInt().AddDependency(EPlayerStatType.Strength, StaticOptions.ADDITIONAL_CAPACITY_PER_STRENGTH_POINT);
            stats[EPlayerStatType.MaxHealth].AsInt().AddDependency(EPlayerStatType.Strength, StaticOptions.MAX_HEALTH_PER_STRENGTH_POINT);
            
            //INTELIGENCE
            stats[EPlayerStatType.CooldownReduction]
                .AsInt()
                .AddDependency(EPlayerStatType.Intelligence, StaticOptions.CDR_PER_INTELLIGENCE_POINT)
                .SetLimit(StaticOptions.MAX_CDR_PERCENTAGE);

            stats[EPlayerStatType.Strength].AsInt();
            stats[EPlayerStatType.Dexterity].AsInt();
            stats[EPlayerStatType.Intelligence].AsInt();
            stats[EPlayerStatType.Health].AsInt();
        }

        public static Dictionary<EPlayerStatType, Stat> EditorDictHelper(Dictionary<EPlayerStatType, float> dict)
        {
            var newDict = new Dictionary<EPlayerStatType, Stat>();
            foreach (var statType in (EPlayerStatType[])System.Enum.GetValues(typeof(EPlayerStatType)))
            {
                var hasValue = dict.TryGetValue(statType, out var current);
                newDict.Add(statType, new Stat(hasValue ? current : 0));
            }
            AddDependencies(newDict);
            return newDict;
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
            
            public float GetDependentValue(Dictionary<EPlayerStatType, Stat> stats)
            {
                foreach (var pair in _dependencies)
                {
                    Value += stats[pair.stat].Value * pair.factor;

                    if (!limit.HasValue || Value < limit) continue;

                    Value = limit.Value;
                    break;
                }

                return _isInt ? Mathf.CeilToInt(Value) : Value;
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