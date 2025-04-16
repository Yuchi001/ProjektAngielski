using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerPack.Editor
{
    [CustomEditor(typeof(InventorySlot))]
    public class CeInventorySlot : UnityEditor.Editor
    {
        private InventorySlot _slot;
        private SerializedProperty _frameImage;
        private SerializedProperty _dragItemColor;

        private void OnEnable()
        {
            _slot = target as InventorySlot;
            _frameImage = serializedObject.FindProperty("frameImage");
            _dragItemColor = serializedObject.FindProperty("dragItemColor");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_frameImage);
            EditorGUILayout.PropertyField(_dragItemColor);
            
            var stateData = new List<InventorySlot.StateData>();
            foreach (var state in _slot.States)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(state.state.ToString(), EditorStyles.boldLabel);
                var data = new InventorySlot.StateData
                {
                    state = state.state,
                    frameColor = EditorGUILayout.ColorField("Frame Color", state.frameColor),
                    itemColor = EditorGUILayout.ColorField("Item Color", state.itemColor)
                };
                stateData.Add(data);
            }
            _slot.SetStateData(stateData);

            EditorGUILayout.Space();
            if (GUILayout.Button("RESET STATE DATA")) _slot.SetStateData(new List<InventorySlot.StateData>());
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("SET DIRTY")) EditorUtility.SetDirty(_slot);
        }
    }
}