using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Structure", menuName = "Custom/Structure")]
    public class SoStructure : ScriptableObject
    {
        [SerializeField] private string structureName;
        [SerializeField, TextArea(3, 10)] private string structureDescription;
        [SerializeField] private GameObject structurePrefab;
        [SerializeField] private bool reusable;

        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public GameObject StructurePrefab => structurePrefab;
        public bool Reusable => reusable;
    }
}