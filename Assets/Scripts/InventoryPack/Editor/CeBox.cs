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

            if (GUILayout.Button("Generate Slots"))
            {
                box.DestroySlots();
                box.SpawnSlots();
                MarkSceneAsDirty();
            }

            if (GUILayout.Button("Destroy Slots"))
            {
                box.DestroySlots();
                MarkSceneAsDirty();
            }
        }
        
        private void MarkSceneAsDirty()
        {
            EditorUtility.SetDirty(box);

            if (Application.isPlaying) return;
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(box.gameObject.scene);
        }
    }
}