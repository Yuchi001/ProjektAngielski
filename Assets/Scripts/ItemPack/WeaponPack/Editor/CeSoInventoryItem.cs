using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using UnityEditor;
using UnityEngine;

namespace ItemPack.WeaponPack.Editor
{
    [CustomEditor(typeof(SoInventoryItem))]
    public class CeSoInventoryItem : UnityEditor.Editor
    {
        private SerializedProperty itemName;
        private SerializedProperty itemDescription;
        private SerializedProperty sprite;
        private SerializedProperty oneTimeSpawnLogic;
        private SerializedProperty itemPrefab;
        private SerializedProperty itemTags;
        private SerializedProperty itemType;
        private SerializedProperty itemPrice;

        private SoInventoryItem _soInventoryItem;
        
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
            _soInventoryItem = target as SoInventoryItem;
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

            var editedStartingStats =  new List<StatPair>();

            var stats = _soInventoryItem.StartingStats;
            if (stats == null) return;

            EditorGUILayout.LabelField("Stats settings", EditorStyles.boldLabel);      
            foreach (var stat in stats)
            {
                if (_soInventoryItem.OneTimeSpawnLogic && stat.SelfStatType == EItemSelfStatType.Cooldown)
                {
                    editedStartingStats.Add(new StatPair(EItemSelfStatType.Cooldown, 0));
                    continue;
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                    
                EditorGUILayout.LabelField(stat.SelfStatType.ToString(), EditorStyles.boldLabel);                
                    
                var val = EditorGUILayout.FloatField("Stat value", stat.StatValue);
                var canUpgrade = EditorGUILayout.Toggle("Can Upgrade", stat.CanUpgrade);
                    
                var current = new StatPair(
                    stat.SelfStatType,
                    val
                );
                    
                if (canUpgrade)
                {
                    float? scaled = null;
                    
                    GUILayout.BeginHorizontal();

                    var useMin = EditorGUILayout.Toggle("Use min", stat.HasMinimum);
                    if (useMin && GUILayout.Button("Scale with min")) scaled = (stat.Minimum - stat.StatValue) / 10;

                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();

                    var useMax = EditorGUILayout.Toggle("Use max", stat.HasMaximum);
                    if (useMax && GUILayout.Button("Scale with max")) scaled = (stat.Maximum - stat.StatValue) / 10;

                    GUILayout.EndHorizontal();

                    current.SetUpgradeStats(new StatPair.UpgradeProps
                    {
                        useMin = useMin,
                        useMax = useMax,
                        min = stat.HasMinimum ? EditorGUILayout.FloatField("Min", stat.Minimum) : 0,
                        max = stat.HasMaximum ? EditorGUILayout.FloatField("Max", stat.Maximum) : 0,
                        mod = scaled ?? EditorGUILayout.FloatField("Mod", stat.Mod),
                    });
                }

                editedStartingStats.Add(current);

                EditorGUI.EndDisabledGroup();
            }

            _soInventoryItem.SetWeaponStartingStats(editedStartingStats);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Restore Defaults"))
            {
                GUIUtility.keyboardControl = 0;
                _soInventoryItem.SetWeaponStartingStats(new List<StatPair>());
            }
            
            serializedObject.ApplyModifiedProperties();
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_soInventoryItem);
        }
    }
}