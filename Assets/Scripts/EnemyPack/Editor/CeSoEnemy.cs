using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States;
using EnemyPack.States.StateData;
using UnityEditor;
using UnityEngine;
using Utils;

namespace EnemyPack.Editor
{
    [CustomEditor(typeof(SoEnemy))]
    public class CeSoEnemy : UnityEditor.Editor
    {
        private SoEnemy _enemy;
        
        private void OnEnable()
        {
            _enemy = (SoEnemy)target;
        }

        public override void OnInspectorGUI()
        {
            BaseInspector();

            if (!_enemy.HasStateData()) return;
            
            if (GUILayout.Button("CLEAR"))
            {
                _enemy.SetStatesData(new List<StateDataBase>());
                var path = AssetDatabase.GetAssetPath(target);
                var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (var asset in assets)
                {
                    DestroyImmediate(asset, true);
                }

                return;
            }
            
            var changed = false;
            var rootState = StateFactory.GetState(_enemy.EnemyBehaviour, null);
            var types = rootState.RequiredDataTypes;

            var list = new List<StateDataBase>();
            foreach (var type in types)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                var obj = _enemy.GetStateData(type);
                var serializedObj = new SerializedObject(obj);
                var iterator = serializedObj.GetIterator();
                var typeStr = type.ToString();
                var startIndex = (typeStr.Contains('+') ? typeStr.LastIndexOf('+') : typeStr.LastIndexOf('.')) + 1;
                var title = typeStr[startIndex..].Replace("Data", "").SplitCamelCase();
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);   
                EditorGUILayout.Space();
                iterator.NextVisible(true);
                
                EditorGUI.BeginChangeCheck();
                while (iterator.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                if (EditorGUI.EndChangeCheck()) changed = true;
                
                serializedObj.ApplyModifiedProperties();
                list.Add(obj);
            }

            if (!changed) return;
            
            _enemy.SetStatesData(list);
            EditorUtility.SetDirty(_enemy);
            AssetDatabase.SaveAssets();
            serializedObject.ApplyModifiedProperties();
            
        }
        
        private void BaseInspector()
        {
            serializedObject.Update();

            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                if (prop.name == "m_Script")
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(prop, true);
                    EditorGUI.EndDisabledGroup();
                    continue;
                }

                if (prop.name == "statesData")
                {
                    continue;
                }

                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}