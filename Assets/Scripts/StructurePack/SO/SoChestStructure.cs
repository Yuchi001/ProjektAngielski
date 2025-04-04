using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventoryPack.WorldItemPack;
using ItemPack.SO;
using PlayerPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Chest Structure", menuName = "Custom/Structure/Chest")]
    public class SoChestStructure : SoStructure
    {
        [SerializeField, Range(1, 100)] private int maxCashReward;
        [SerializeField, Range(1, 4)] private int maxWeaponCount;

        private static IEnumerator SpawnItems(List<SoInventoryItem> items, Vector2 position)
        {
            foreach (var item in items)
            {
                WorldItemManager.SpawnInventoryItem(item, position, 1);
                yield return new WaitForFixedUpdate();
            }
        }

        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            var resultItems = new List<SoInventoryItem>();
            var calculatedCash = Random.Range(0, maxCashReward);
            var items = PlayerManager.PlayerItemManager.GetRandomItems(maxWeaponCount);
            foreach (var item in items)
            {
                resultItems.Add(item);
                if (resultItems.Sum(i => i.ItemPrice) + calculatedCash > item.ItemPrice) break;
            }

            var spawnPos = structureBase.transform.position;
            WorldItemManager.SpawnCoins(calculatedCash, spawnPos);
            structureBase.StartCoroutine(SpawnItems(resultItems, spawnPos));
            
            Destroy(structureBase, 10f);

            return true;
        }
    }
}