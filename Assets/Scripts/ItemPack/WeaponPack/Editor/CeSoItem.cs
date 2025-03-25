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

        private SoItem _soItem;

        private static readonly Dictionary<int, bool> foldouts = new()
        {
            { 0 , false },
            { 1 , false },
            { 2 , false },
        };

        private bool _startingStatsFoldout = false;
        
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
            _soItem = target as SoItem;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(itemName);
            EditorGUILayout.PropertyField(itemDescription);
            EditorGUILayout.PropertyField(itemPrice);
            EditorGUILayout.PropertyField(itemType);
            EditorGUILayout.PropertyField(sprite);
            EditorGUILayout.PropertyField(oneTimeSpawnLogic);
            EditorGUILayout.PropertyField(itemPrefab);
            EditorGUILayout.PropertyField(itemTags);
            
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
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    
                    var statType = (EItemSelfStatType)EditorGUILayout.EnumPopup("Stat type", stat.SelfStatType);
                    EditorGUI.BeginDisabledGroup(statType == EItemSelfStatType.None);
                    
                    var val = EditorGUILayout.FloatField("Stat value", stat.StatValue);
                    var canUpgrade = EditorGUILayout.Toggle("Can Upgrade", stat.CanUpgrade);
                    
                    var current = new StatPair(
                        statType,
                        val
                        );

                    if (canUpgrade) current.SetUpgradeStats(new StatPair.UpgradeProps
                    {
                        min = EditorGUILayout.FloatField("Min", stat.Minimum),
                        max = EditorGUILayout.FloatField("Max", stat.Maximum),
                        mod = EditorGUILayout.FloatField("Mod", stat.Mod),
                        percentageGrowth = EditorGUILayout.Toggle("Percentage Growth", stat.PercentageGrowth),
                    });
                    
                    if (GUILayout.Button("Remove")) RemoveStartingStat(index);
                    else editedStartingStats.Add(current);
                    
                    EditorGUI.EndDisabledGroup();
                }
                _soItem.SetWeaponStartingStats(editedStartingStats);
            
                serializedObject.ApplyModifiedProperties();
            }
            
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
    }
}