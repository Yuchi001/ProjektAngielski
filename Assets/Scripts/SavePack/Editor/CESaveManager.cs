using UnityEditor;
using UnityEngine;

namespace SavePack.Editor
{
    [CustomEditor(typeof(SaveManager))]
    public class CESaveManager : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Delete Save File")) FileManager.DeletePlayerData();
        }
    }
}