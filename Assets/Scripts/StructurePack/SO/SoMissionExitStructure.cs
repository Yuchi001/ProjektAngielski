using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventoryPack.WorldItemPack;
using ItemPack.SO;
using Managers;
using MapPack;
using PlayerPack;
using UnityEngine;
using Utils;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission Exit Structure", menuName = "Custom/Structure/MissionExit")]
    public class SoMissionExitStructure : SoStructure
    {
        [Header("Mission Exit Options")]
        [SerializeField] private MinMax weaponPerCoin;
        [SerializeField] private float waitBeforeReturn;
        
        public override bool OnInteract(StructureBase structureBase)
        {
            var data = structureBase.GetData<MissionExitData>();
            if (!data.IsInitialized()) throw new NullReferenceException("MISSION NOT SET FOR MISSION EXIT!");

            if (structureBase.WasUsed)
            {
                structureBase.StartCoroutine(ReturnToMap());
                return true;
            }
            
            if (PlayerManager.PlayerSoulManager.GetCurrentSoulCount() < data.SoulCount) return false;

            PlayerManager.StartPlayerExitSequence(() => RewardPlayer(structureBase));
            structureBase.SetCanInteract(false);
            
            return true;
        }

        private IEnumerator ReturnToMap()
        {
            PlayerManager.PlayerHealth.Invincible = true;
            PlayerManager.LockKeys();
            //TODO: Animacja znikania gracza
            //TODO: dźwięk znikania gracza
            yield return new WaitForSeconds(waitBeforeReturn);
            GameManager.ReturnToMap();
        }

        private void RewardPlayer(StructureBase structureBase)
        {
            var data = structureBase.GetData<MissionExitData>();
            if (!data.IsInitialized()) throw new NullReferenceException("MISSION NOT SET FOR MISSION EXIT!");

            var rewardSum = data.BaseReward;
            rewardSum += PlayerManager.PlayerSoulManager.GetCurrentSoulCount() * data.SoulToCoinRatio;

            var rewardItems = new List<SoInventoryItem>();
            var weaponCount = (int)(weaponPerCoin.RandomFloat() * rewardSum);
            var items = PlayerManager.PlayerItemManager.GetRandomItems(weaponCount);
            foreach (var item in items)
            {
                if (rewardItems.Sum(i => i.ItemPrice) + item.ItemPrice >= rewardSum) break;
                rewardItems.Add(item);
            }

            var cashInItems = rewardItems.Sum(i => i.ItemPrice);
            var coinReward = rewardSum - cashInItems;

            var spawnPos = structureBase.transform.position;
            if (coinReward > 0) WorldItemManager.SpawnCoins(coinReward, spawnPos);
            foreach (var item in rewardItems) WorldItemManager.SpawnInventoryItem(item, spawnPos, 1);
            
            structureBase.SetCanInteract(true);
        }

        public override string GetInteractionMessage(StructureBase structureBase)
        {
            var data = structureBase.GetData<MissionExitData>();
            if (!data.IsInitialized()) throw new NullReferenceException("MISSION NOT SET FOR MISSION EXIT!");

            if (structureBase.WasUsed) return "Exit";
            return base.GetInteractionMessage(structureBase).Replace("$x$", data.SoulCount.ToString());
        }

        public void SetMission(StructureBase structureBase, MapManager.MissionData missionData)
        {
            var data = structureBase.GetData<MissionExitData>();
            data.Init().SetMission(missionData);
        }

        private class MissionExitData : BaseStructureData<MissionExitData>
        {
            private MapManager.MissionData _currenMission;

            public void SetMission(MapManager.MissionData currentMission)
            {
                _currenMission = currentMission;
            }

            public int SoulCount => _currenMission.SoulCount;

            public int BaseReward => _currenMission.CoinReward;

            public int SoulToCoinRatio => _currenMission.SoulToCoinRatio;
        }
    }
}