using System.Collections;
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
            var calculatedCash = Random.Range(0, maxCashReward);
            var items = PlayerManager.Instance.PlayerItemManager.GetRandomItems(maxWeaponCount);
            foreach (var item in items)
            {
                resultItems.Add(item);
                if (resultItems.Sum(i => i.ItemPrice) + calculatedCash > item.ItemPrice) break;
            }

            var spawnPos = structureBase.transform.position;
            WorldItemManager.SpawnCoins(calculatedCash, spawnPos);
            structureBase.StartCoroutine(SpawnItems(resultItems, spawnPos));
            
            Destroy(structureBase, 10f);
        }

        private IEnumerator SpawnItems(List<SoItem> items, Vector2 position)
        {
            foreach (var item in items)
            {
                WorldItemManager.SpawnItem(item, 1, position);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}