using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using ItemPack.WeaponPack.SideClasses;
using Managers;
using PlayerPack.Enums;
using UnityEditor;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.Editor
{
    [CustomEditor(typeof(SoItem))]
    public class CeSoItem : UnityEditor.Editor
    {
        private SerializedProperty itemName;
        private SerializedProperty itemDescription;
        private SerializedProperty sprite;
        private SerializedProperty oneTimeSpawnLogic;
        private SerializedProperty itemPrefab;
        private SerializedProperty itemTags;
        private SerializedProperty itemType;
        private SerializedProperty itemPrice;
        private SerializedProperty itemWeight;

        private SoItem _soItem;

        private static readonly Dictionary<int, bool> foldouts = new()
        {
            { 0 , false },
            { 1 , false },
            { 2 , false },
        };

        private bool _startingStatsFoldout = false;
        private bool _scalingFoldout = false;
        
        private void OnEnable()
        {
            itemName = serializedObject.FindProperty("itemName");
            itemDescription = serializedObject.FindProperty("itemDescription");
            itemPrice = serializedObject.FindProperty("itemPrice");
            sprite = serializedObject.FindProperty("itemSprite");
            oneTimeSpawnLogic = serializedObject.FindProperty("oneTimeSpawnLogic");
            itemPrefab = serializedObject.FindProperty("itemPrefab");
            itemTags = serializedObject.FindProperty("itemTags");
            itemType = serializedObject.FindProperty("itemType");
            itemWeight = serializedObject.FindProperty("itemWeight");
            _soItem = target as SoItem;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(itemName);
            EditorGUILayout.PropertyField(itemDescription);
            EditorGUILayout.PropertyField(itemWeight);
            EditorGUILayout.PropertyField(itemPrice);
            EditorGUILayout.PropertyField(itemType);
            EditorGUILayout.PropertyField(sprite);
            EditorGUILayout.PropertyField(oneTimeSpawnLogic);
            EditorGUILayout.PropertyField(itemPrefab);
            EditorGUILayout.PropertyField(itemTags);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            try
            {
                _scalingFoldout = EditorGUILayout.Foldout(_scalingFoldout, "Scaling power");
                if (_scalingFoldout)
                {
                    var statScalePairs = new List<SoItem.StatScalePair>();
                    foreach (var playerStatType in (EPlayerStatType[])System.Enum.GetValues(typeof(EPlayerStatType)))
                    {
                        EditorGUILayout.BeginHorizontal();
                        var scalingPower = (EScalingPower)EditorGUILayout.EnumPopup(playerStatType.ToString(),
                            _soItem.ItemScalingPower[playerStatType]);
                        EditorGUILayout.EndHorizontal();
                        statScalePairs.Add(new SoItem.StatScalePair(playerStatType, scalingPower));
                    }

                    _soItem.SetScalingPower(statScalePairs);
                }
            }
            catch
            {
                _soItem.InitItemScaling();
            }
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            _startingStatsFoldout = EditorGUILayout.Foldout(_startingStatsFoldout, "Starting stats");
            
            EditorGUI.BeginDisabledGroup(_soItem.StartingStats.FirstOrDefault(s => s.SelfStatType == EItemSelfStatType.None) != default);
            if (GUILayout.Button("Add new stat")) AddStartingStat();
            EditorGUI.EndDisabledGroup();
            
            GUILayout.EndHorizontal();
            
            if (_startingStatsFoldout)
            {
                var editedStartingStats =  new List<StatPair>();

                foreach (var (stat, index) in _soItem.StartingStats.Select((item, index) => (item, index)))
                {
                    var statType = (EItemSelfStatType)EditorGUILayout.EnumPopup("Stat type", stat.SelfStatType);
                    EditorGUI.BeginDisabledGroup(statType == EItemSelfStatType.None);
                    var val = EditorGUILayout.FloatField("Stat value", stat.StatValue);
                    var isPercentage = EditorGUILayout.Toggle("Is percentage", stat.IsPercentage);
                    var canUpgrade = EditorGUILayout.Toggle("Can Upgrade", stat.CanUpgrade);
                    EditorGUI.EndDisabledGroup();
                    
                    if (GUILayout.Button("Remove")) RemoveStartingStat(index);
                    else
                    {
                        var current = new StatPair(
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
                _soItem.SetWeaponStartingStats(editedStartingStats);
            
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            HandleGenerateTieredStats();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            var tieredStatsDictionary = new Dictionary<int, List<StatPair>>();

            EditorGUILayout.LabelField("Upgrade stats", EditorStyles.boldLabel);
            foreach (var tieredWeaponStat in _soItem.TieredStats.Dict)
            {
                foldouts[tieredWeaponStat.Key] = EditorGUILayout.Foldout(foldouts[tieredWeaponStat.Key],
                    $"Level: {tieredWeaponStat.Key.ToString()}");
                var stats = foldouts[tieredWeaponStat.Key] ? new List<StatPair>() : tieredWeaponStat.Value;
                if (foldouts[tieredWeaponStat.Key])
                {
                    foreach (var (value, index) in tieredWeaponStat.Value.Select((item, index) => (item, index)))
                    {
                        if (value.SelfStatType == EItemSelfStatType.None) continue;
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(!value.CanUpgrade || tieredWeaponStat.Key == 0);
                        var val = EditorGUILayout.FloatField(value.SelfStatType.ToString(), value.StatValue);
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.LabelField($"Calculated value: {value.StatValue + _soItem.StartingStats[index].StatValue}", EditorStyles.whiteMiniLabel);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        var updatedWeaponStat = new StatPair(value.SelfStatType, val, value.IsPercentage, value.CanUpgrade);
                        stats.Add(updatedWeaponStat);
                    }

                }

                tieredStatsDictionary.Add(tieredWeaponStat.Key, stats);
            }
            
            _soItem.SetWeaponTieredStats(tieredStatsDictionary);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_soItem);
        }

        private void AddStartingStat()
        {
            var current = new List<StatPair>(_soItem.StartingStats) { new() };
            _soItem.SetWeaponStartingStats(current);
            _startingStatsFoldout = true;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void RemoveStartingStat(int index)
        {
            var current = new List<StatPair>(_soItem.StartingStats);
            current.RemoveAt(index);
            _soItem.SetWeaponStartingStats(current);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void HandleGenerateTieredStats()
        {
            var uniqueItems = _soItem.StartingStats.Distinct(new WeaponStatComparer()).ToList();
            _soItem.SetWeaponStartingStats(uniqueItems);
            
            serializedObject.ApplyModifiedProperties();

            var currentTieredStats = _soItem.TieredStats.Dict;

            if (currentTieredStats.Any())
            {
                var dictStatusKeys = currentTieredStats[0].Select(e => e.SelfStatType).ToList();
                var startingStatusKeys = uniqueItems.Select(e => e.SelfStatType).ToList();

                var invalidKeys = dictStatusKeys.Except(startingStatusKeys).ToList();
                
                currentTieredStats.Keys.ToList()
                    .ForEach(k => currentTieredStats[k].RemoveAll(e => invalidKeys.Contains(e.SelfStatType)));

                if (currentTieredStats[1].Count <= 0)
                    currentTieredStats = new Dictionary<int, List<StatPair>>();
            }

            foreach (var (statPair, index) in uniqueItems.Select((element, index) => (element, index)))
            {
                foreach (var (tier, stats) in currentTieredStats.Select(dictPair => (dictPair.Key, dictPair.Value)))
                {
                    if (stats.Count > index)
                    {
                        var value = stats[index].StatValue;
                        stats[index] = new StatPair(statPair, statPair.CanUpgrade && tier != 0 ? value : 0, false);
                    }
                    else stats.Add(statPair);
                }
            }
            
            var tieredStatsDictionary = new Dictionary<int, List<StatPair>>();
            for (var level = 0; level <= StaticOptions.MAX_TIER; level++)
            {
                var stats = !currentTieredStats.TryGetValue(level, out var currentStats) ? 
                    _soItem.StartingStats.Select(pair => new StatPair(pair)).ToList() : currentStats;
                tieredStatsDictionary.Add(level, stats);
            }
            
            _soItem.SetWeaponTieredStats(tieredStatsDictionary);

            serializedObject.ApplyModifiedProperties();
        }

        private class WeaponStatComparer : IEqualityComparer<StatPair>
        {
            public bool Equals(StatPair x, StatPair y)
            {
                return x?.SelfStatType == y?.SelfStatType;
            }

            public int GetHashCode(StatPair obj)
            {
                return obj.SelfStatType.GetHashCode();
            }
        }
    }
}