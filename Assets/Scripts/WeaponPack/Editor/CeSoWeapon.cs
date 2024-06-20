using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private SerializedProperty weaponColor;
        private SerializedProperty oneTimeSpawnLogic;
        private SerializedProperty weaponLogicPrefab;
        private SerializedProperty maxLevelPrize;
        private SerializedProperty weaponStartingStats;

        private SoWeapon _soWeapon;
        
        private void OnEnable()
        {
            weaponName = serializedObject.FindProperty("weaponName");
            weaponDescription = serializedObject.FindProperty("weaponDescription");
            weaponSprite = serializedObject.FindProperty("weaponSprite");
            weaponColor = serializedObject.FindProperty("weaponColor"); 
            oneTimeSpawnLogic = serializedObject.FindProperty("oneTimeSpawnLogic");
            weaponLogicPrefab = serializedObject.FindProperty("weaponLogicPrefab");
            maxLevelPrize = serializedObject.FindProperty("maxLevelPrize");
            weaponStartingStats = serializedObject.FindProperty("weaponStartingStats");
            _soWeapon = target as SoWeapon;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(weaponName);
            EditorGUILayout.PropertyField(weaponDescription);
            EditorGUILayout.PropertyField(weaponSprite);
            EditorGUILayout.PropertyField(weaponColor);
            EditorGUILayout.PropertyField(oneTimeSpawnLogic);
            EditorGUILayout.PropertyField(weaponLogicPrefab);
            EditorGUILayout.PropertyField(maxLevelPrize);
            EditorGUILayout.PropertyField(weaponStartingStats);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate upgrade stats")) HandleGenerateUpdateStats();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            var upgradeStats = new List<UpgradeWeaponStat>();
            foreach (var upgradeWeaponStat in _soWeapon.WeaponUpgradeStats)
            {
                EditorGUILayout.LabelField(upgradeWeaponStat.StatType.ToString(), EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Description");
                var desc = EditorGUILayout.TextArea(upgradeWeaponStat.RawDescription, GUILayout.Height(40));
                var val = EditorGUILayout.FloatField("Stat value", upgradeWeaponStat.StatValue);
                var growth = EditorGUILayout.FloatField("Value growth per level", upgradeWeaponStat.ValueGrowth);
                var levelCount = EditorGUILayout.IntField("Level count", upgradeWeaponStat.ValueLevels);
                var isPercentage = EditorGUILayout.Toggle("Is percentage", upgradeWeaponStat.IsPercentage);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                    
                var updatedWeaponStat = new UpgradeWeaponStat(
                    val, upgradeWeaponStat.StatType, isPercentage, desc, levelCount, growth
                );
                upgradeStats.Add(updatedWeaponStat);
            }
            _soWeapon.SetWeaponUpgradeStats(upgradeStats);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_soWeapon);
        }

        private void HandleGenerateUpdateStats()
        {
            var upgradeStats = new List<UpgradeWeaponStat>();
            foreach (var pair in _soWeapon.WeaponStartingStats)
            {
                if(!pair.CanUpgrade) continue;
                
                var current = _soWeapon.WeaponUpgradeStats.FirstOrDefault(w => w.StatType == pair.StatType);
                    
                upgradeStats.Add(current ?? new UpgradeWeaponStat(pair));
            }
            _soWeapon.SetWeaponUpgradeStats(upgradeStats);

            serializedObject.ApplyModifiedProperties();
        }
    }
}