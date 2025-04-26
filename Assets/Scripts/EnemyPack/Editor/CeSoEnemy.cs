using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States;
using EnemyPack.States.StateData;
using UnityEditor;
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
            //base.OnInspectorGUI();
            BaseInspector();
            
            var rootState = StateFactory.GetState(_enemy.EnemyBehaviour);
            var types = rootState.RequiredDataTypes;

            var list = new List<StateDataBase>();
            foreach (var type in types)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                var obj = _enemy.GetStateData(type) ?? (StateDataBase)CreateInstance(type);
                var serializedObj = new SerializedObject(obj);
                var iterator = serializedObj.GetIterator();
                var typeStr = type.ToString();
                var startIndex = typeStr.LastIndexOf('.') + 1;
                var title = typeStr.Contains('+') ? "Base State" : typeStr[startIndex..].Replace("Data", "").SplitCamelCase();
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);   
                EditorGUILayout.Space();
                iterator.NextVisible(true);
                while (iterator.NextVisible(false))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                serializedObj.ApplyModifiedProperties();
                list.Add(obj);
            }
            _enemy.SetStatesData(list);
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_enemy);
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