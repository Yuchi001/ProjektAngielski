using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WeaponPack.SideClasses;
using WeaponPack.SO;

namespace WeaponPack.Editor
{
    [CustomEditor(typeof(SoWeapon))]
    public class CeSoWeapon : UnityEditor.Editor
    {
        private SoWeapon _soWeapon;
        
        private void OnEnable()
        {
            _soWeapon = target as SoWeapon;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate upgrade stats")) HandleGenerateUpdateStats();

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
        }

        private void HandleGenerateUpdateStats()
        {
            var upgradeStats = new List<UpgradeWeaponStat>();
            foreach (var pair in _soWeapon.WeaponStartingStats)
            {
                var current = _soWeapon.WeaponUpgradeStats.FirstOrDefault(w => w.StatType == pair.StatType);
                    
                upgradeStats.Add(current ?? new UpgradeWeaponStat(pair));
            }
            _soWeapon.SetWeaponUpgradeStats(upgradeStats);

            serializedObject.ApplyModifiedProperties();
        }
    }
}