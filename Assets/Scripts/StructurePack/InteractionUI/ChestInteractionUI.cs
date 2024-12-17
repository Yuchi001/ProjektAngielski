using InventoryPack;
using PlayerPack;
using StructurePack.SO;
using UnityEngine;

namespace StructurePack.InteractionUI
{
    public class ChestInteractionUI : Box, IStructure
    {
        private PlayerItemManager PlayerItemManager => PlayerManager.Instance.PlayerItemManager;
        
        protected override void Init()
        {
            
        }

        public override void Close()
        {
            PlayerManager.Instance.PlayerItemManager.RefreshInventory();
            Time.timeScale = 1;
            Destroy(gameObject);
        }

        public override void Open()
        {
            base.Open();
            Time.timeScale = 0;
        }

        public void Setup(SoStructure structureData)
        {
            if (PlayerManager.Instance == null) return;

            var chest = (SoChestStructure)structureData;
            var itemCount = Random.Range(1, chest.MaxItems + 1);
            foreach (var item in PlayerItemManager.GetRandomItems(itemCount, chest.RandomPercentageBust))
            {
                Debug.Log("ADDING ITEM");
                AddItem(item, 1);
            }
            
            Open();
        }
    }
}