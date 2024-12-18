using UnityEditor;
using UnityEngine;

namespace InventoryPack.Editor
{
    [CustomEditor(typeof(Box), true)]
    public class CeBox : UnityEditor.Editor
    {
        private Box box;

        private void OnEnable()
        {
            box = target as Box;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate Slots")) box.SpawnSlots();
        }
    }
}