using System;
using System.Collections.Generic;
using System.Linq;
using PlayerPack.Enums;
using PlayerPack.SO;
using UnityEditor;
using UnityEditor.UIElements;

namespace PlayerPack.Editor
{
    [CustomEditor(typeof(SoCharacter))]
    public class CeSoCharacter : UnityEditor.Editor
    {
        private SerializedProperty characterName;
        private SerializedProperty characterSprite;
        private SerializedProperty walkingAnim;
        private SerializedProperty idleAnim;
        private SerializedProperty characterColor;
        private SerializedProperty startingItem;

        private SoCharacter _soCharacter;

        private void OnEnable()
        {
            _soCharacter = target as SoCharacter;
            
            characterName = serializedObject.FindProperty("characterName");
            characterSprite = serializedObject.FindProperty("characterSprite");
            walkingAnim = serializedObject.FindProperty("walkingAnim");
            idleAnim = serializedObject.FindProperty("idleAnim");
            characterColor = serializedObject.FindProperty("characterColor");
            startingItem = serializedObject.FindProperty("startingItem");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(characterName);
            EditorGUILayout.PropertyField(characterSprite);
            EditorGUILayout.PropertyField(walkingAnim);
            EditorGUILayout.PropertyField(idleAnim);
            EditorGUILayout.PropertyField(characterColor);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            var stats = new List<SoCharacter.PlayerStatPair>();
            var dictHelper = PlayerStatsManager.EditorDictHelper(_soCharacter.StatDict);
            foreach (var statType in (EPlayerStatType[])System.Enum.GetValues(typeof(EPlayerStatType)))
            {
                if (statType == EPlayerStatType.Health)
                {
                    stats.Add(new SoCharacter.PlayerStatPair(statType, 0));
                    continue;
                }
                
                var hasValue = _soCharacter.StatDict.TryGetValue(statType, out var current);

                EditorGUILayout.BeginHorizontal();
                var value = EditorGUILayout.FloatField(statType + $" |{PlayerStatUtils.DependencyToStr(statType)}|", hasValue ? current : 0);
                EditorGUILayout.LabelField($"Calculated value: {dictHelper[statType].GetDependentValue(dictHelper)} {Dependencies(dictHelper[statType])}", EditorStyles.whiteMiniLabel);
                EditorGUILayout.EndHorizontal();
                
                stats.Add(new SoCharacter.PlayerStatPair(statType, value));
            }
            _soCharacter.SetStats(stats);
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(startingItem);

            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_soCharacter);
        }

        private string Dependencies(PlayerStatsManager.Stat stat)
        {
            var dependencies = stat.GetDependencies();
            return dependencies.Any() ? $"|{PlayerStatUtils.DependenciesToStr(dependencies)}|" : "";
        }
    }
}