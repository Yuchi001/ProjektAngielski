using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeaponPack.Enums;
using WeaponPack.SideClasses;
using WeaponPack.SO;

namespace WeaponPack.Editor
{
    [CustomEditor(typeof(SoWeapon))]
    public class CeSoWeapon : UnityEditor.Editor
    {
        private SerializedProperty weaponName;
        private SerializedProperty weaponDescription;
        private SerializedProperty weaponSprite;
        private SerializedProperty oneTimeSpawnLogic;
        private SerializedProperty weaponLogicPrefab;
        private SerializedProperty weaponCategory;
        private SerializedProperty weaponStartingStats;

        private SoWeapon _soWeapon;

        private static Dictionary<ETierType, bool> foldouts = new Dictionary<ETierType, bool>()
        {
            { ETierType.Common , false },
            { ETierType.Epic , false },
            { ETierType.Rare , false },
        };

        private bool _startingStatsFoldout = false;
        
        private void OnEnable()
        {
            weaponName = serializedObject.FindProperty("weaponName");
            weaponDescription = serializedObject.FindProperty("weaponDescription");
            weaponSprite = serializedObject.FindProperty("weaponSprite");
            oneTimeSpawnLogic = serializedObject.FindProperty("oneTimeSpawnLogic");
            weaponLogicPrefab = serializedObject.FindProperty("weaponLogicPrefab");
            weaponCategory = serializedObject.FindProperty("weaponCategory");
            weaponStartingStats = serializedObject.FindProperty("weaponStartingStats");
            _soWeapon = target as SoWeapon;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(weaponName);
            EditorGUILayout.PropertyField(weaponDescription);
            EditorGUILayout.PropertyField(weaponSprite);
            EditorGUILayout.PropertyField(oneTimeSpawnLogic);
            EditorGUILayout.PropertyField(weaponLogicPrefab);
            EditorGUILayout.PropertyField(weaponCategory);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            _startingStatsFoldout = EditorGUILayout.Foldout(_startingStatsFoldout, "Starting stats");
            if (GUILayout.Button("Add new stat")) AddStartingStat();
            GUILayout.EndHorizontal();
            
            if (_startingStatsFoldout)
            {
                var editedStartingStats =  new List<WeaponStatPair>();

                foreach (var (stat, index) in _soWeapon.WeaponStartingStats.Select((item, index) => (item, index)))
                {
                    var statType = (EWeaponStat)EditorGUILayout.EnumPopup("Stat type", stat.StatType);
                    var val = EditorGUILayout.FloatField("Stat value", stat.StatValue);
                    var isPercentage = EditorGUILayout.Toggle("Is percentage", stat.IsPercentage);
                    var canUpgrade = EditorGUILayout.Toggle("Can Upgrade", stat.CanUpgrade);
                
                    if (GUILayout.Button("Remove")) RemoveStartingStat(index);
                    else
                    {
                        var current = new WeaponStatPair(
                            statType,
                            val,
                            isPercentage,
                            canUpgrade
                        );
                        editedStartingStats.Add(current);
                    }
                
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
                _soWeapon.SetWeaponStartingStats(editedStartingStats);
            
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Refresh tiered stats")) HandleGenerateTieredStats();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            var tieredStatsDictionary = new Dictionary<ETierType, List<WeaponStatPair>>();

            try
            {
                foreach (var tieredWeaponStat in _soWeapon.WeaponTieredStats.Dict)
                {
                    foldouts[tieredWeaponStat.Key] = EditorGUILayout.Foldout(foldouts[tieredWeaponStat.Key],
                        tieredWeaponStat.Key.ToString());
                    var stats = foldouts[tieredWeaponStat.Key] ? new List<WeaponStatPair>() : tieredWeaponStat.Value;
                    if (foldouts[tieredWeaponStat.Key])
                    {
                        foreach (var (value, index) in tieredWeaponStat.Value.Select((item, index) => (item, index)))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.BeginDisabledGroup(!value.CanUpgrade || tieredWeaponStat.Key == ETierType.Common);
                            var val = EditorGUILayout.FloatField(value.StatType.ToString(), value.StatValue);
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.FloatField(value.StatValue + _soWeapon.WeaponStartingStats[index].StatValue,
                                EditorStyles.whiteMiniLabel);
                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.EndHorizontal();

                            var updatedWeaponStat = new WeaponStatPair(
                                value.StatType, val, value.IsPercentage, value.CanUpgrade
                            );
                            stats.Add(updatedWeaponStat);
                        }

                    }

                    tieredStatsDictionary.Add(tieredWeaponStat.Key, stats);
                }
            }
            catch (Exception e)
            {
                HandleGenerateTieredStats();
            }
            
            _soWeapon.SetWeaponTieredStats(tieredStatsDictionary);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_soWeapon);
        }

        private void AddStartingStat()
        {
            var current = new List<WeaponStatPair>(_soWeapon.WeaponStartingStats) { new() };
            _soWeapon.SetWeaponStartingStats(current);
            _startingStatsFoldout = true;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void RemoveStartingStat(int index)
        {
            var current = new List<WeaponStatPair>(_soWeapon.WeaponStartingStats);
            current.RemoveAt(index);
            _soWeapon.SetWeaponStartingStats(current);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void HandleGenerateTieredStats()
        {
            var uniqueItems = _soWeapon.WeaponStartingStats.Distinct(new WeaponStatComparer()).ToList();
            _soWeapon.SetWeaponStartingStats(uniqueItems);
            
            serializedObject.ApplyModifiedProperties();

            var currentTieredStats = _soWeapon.WeaponTieredStats.Dict;

            if (currentTieredStats.Any())
            {
                var dictStatusKeys = currentTieredStats[ETierType.Common].Select(e => e.StatType).ToList();
                var startingStatusKeys = uniqueItems.Select(e => e.StatType).ToList();

                var invalidKeys = dictStatusKeys.Except(startingStatusKeys).ToList();

                currentTieredStats.Keys.ToList()
                    .ForEach(k => currentTieredStats[k].RemoveAll(e => invalidKeys.Contains(e.StatType)));

                if (currentTieredStats[ETierType.Common].Count <= 0)
                    currentTieredStats = new Dictionary<ETierType, List<WeaponStatPair>>();
            }
            
            
            var tieredStatsDictionary = new Dictionary<ETierType, List<WeaponStatPair>>();
            foreach (var tierType in (ETierType[])System.Enum.GetValues(typeof(ETierType)))
            {
                var stats = !currentTieredStats.TryGetValue(tierType, out var currentStats) ? 
                    _soWeapon.WeaponStartingStats.Select(pair => new WeaponStatPair(pair)).ToList() : currentStats;
                tieredStatsDictionary.Add(tierType, stats);
            }
            _soWeapon.SetWeaponTieredStats(tieredStatsDictionary);

            serializedObject.ApplyModifiedProperties();
            
            foldouts.Keys.ToList().ForEach(k => foldouts[k] = true);
        }

        class WeaponStatComparer : IEqualityComparer<WeaponStatPair>
        {
            public bool Equals(WeaponStatPair x, WeaponStatPair y)
            {
                return x?.StatType == y?.StatType;
            }

            public int GetHashCode(WeaponStatPair obj)
            {
                return obj.StatType.GetHashCode();
            }
        }
    }
}