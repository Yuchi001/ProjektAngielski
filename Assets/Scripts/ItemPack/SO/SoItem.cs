using System.Collections.Generic;
using ItemPack.Enums;
using UnityEngine;

namespace ItemPack.SO
{
    public abstract class SoItem : ScriptableObject
    {
        [SerializeField] protected string itemName;
        [SerializeField, TextArea] protected string itemDescription;
        [SerializeField] protected int itemPrice;
        [SerializeField] protected Sprite itemSprite;
        [SerializeField] protected List<EItemTag> itemTags;
        [SerializeField] protected EItemType itemType;
        [SerializeField] protected float worldLifeTime;
        
        public string ItemName => itemName;
        public int ItemPrice => itemPrice;
        public string ItemDescription => itemDescription;
        public List<EItemTag> ItemTags => itemTags;
        public EItemType ItemType => itemType;
        public Sprite ItemSprite => itemSprite;
        public float WorldLifeTime => worldLifeTime;

        public abstract bool OnPickUp(params int[] paramArray);

        public virtual bool CanPickUp()
        {
            return true;
        }

        public bool Is(SoItem item)
        {
            return itemName == item.itemName;
        }
    }
}