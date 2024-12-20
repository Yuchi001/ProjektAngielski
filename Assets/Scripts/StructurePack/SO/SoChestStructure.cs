using System.Collections.Generic;
using System.Linq;
using InventoryPack.WorldItemPack;
using ItemPack.SO;
using PlayerPack;
using UnityEngine;

namespace StructurePack.SO
{
    public class SoChestStructure : SoStructure
    {
        [SerializeField, Range(1, 100)] private int maxCashReward;
        [SerializeField, Range(1, 4)] private int maxWeaponCount;
        public override void OnInteract(StructureBase structureBase, ref GameObject spawnedInteraction)
        {
            var resultItems = new List<SoItem>();
            var calculatedCash = Random.Range(0, maxCashReward / maxWeaponCount);
            var items = PlayerManager.Instance.PlayerItemManager.GetRandomItems(maxWeaponCount);
            foreach (var item in items)
            {
                resultItems.Add(item);
                if (resultItems.Sum(i => i.ItemPrice) + calculatedCash > item.ItemPrice) break;
            }
            
            WorldItemManager.SpawnCoins(calculatedCash, structureBase.transform.position);

            foreach (var item in resultItems) WorldItemManager.SpawnItem(item, 1, structureBase.transform.position);
            
            Destroy(structureBase, 10f);
        }
    }
}