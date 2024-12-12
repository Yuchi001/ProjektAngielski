using UnityEngine;

namespace StructurePack.SO
{
    public abstract class SoStructure : ScriptableObject
    {
        [SerializeField] private string structureName;
        [SerializeField, TextArea(3, 10)] private string structureDescription;
        [SerializeField] private Sprite structureSprite;
        [SerializeField] private float structureScale = 1;
        [SerializeField] private bool reusable = true;

        public float StructureScale => structureScale;
        public string StructureName => structureName;
        public string StructureDescription => structureDescription;
        public Sprite StructureSprite => structureSprite;
        public bool Reusable => reusable;

        public abstract void OnInteract(StructureBase structureBase);
    }
}