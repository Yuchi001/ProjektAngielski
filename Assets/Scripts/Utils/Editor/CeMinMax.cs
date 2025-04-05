using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var minProp = property.FindPropertyRelative("min");
            var maxProp = property.FindPropertyRelative("max");

            var labelWidth = 28f;
            var fieldWidth = (position.width - labelWidth * 2 - 4f) / 2f;
            var minLabelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var minFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, position.height);
            var maxLabelRect = new Rect(position.x + labelWidth + fieldWidth + 2f, position.y, labelWidth, position.height);
            var maxFieldRect = new Rect(position.x + labelWidth * 2 + fieldWidth + 2f, position.y, fieldWidth, position.height);

            EditorGUI.LabelField(minLabelRect, "Min");
            EditorGUI.PropertyField(minFieldRect, minProp, GUIContent.none);
            EditorGUI.LabelField(maxLabelRect, "Max");
            EditorGUI.PropertyField(maxFieldRect, maxProp, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}