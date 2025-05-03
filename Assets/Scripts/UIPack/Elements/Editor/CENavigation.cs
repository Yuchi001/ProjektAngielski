using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UIPack.Elements.Editor
{
    [CustomEditor(typeof(Navigation))]
    public class CENavigation : UnityEditor.Editor
    {
        private Navigation _navigation;

        private void OnEnable()
        {
            _navigation = target as Navigation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Collect Navigation")) _navigation.SetNavigationElements(_navigation.GetComponentsInChildren<NavigationElement>().ToList());
            
            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(_navigation);
        }
    }
}