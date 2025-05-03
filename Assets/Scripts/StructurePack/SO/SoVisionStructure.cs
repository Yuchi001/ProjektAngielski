using Managers;
using MinimapPack;
using MinimapPack.Strategies;
using PlayerPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using Utils;
using WorldGenerationPack;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Vision Structure", menuName = "Custom/Structure/Vision")]
    public class SoVisionStructure : SoStructure
    {
        [SerializeField] private int baseCost;
        [SerializeField] private int stageBaseMultiplier;
        [SerializeField] private int transactionMultiplier;
        [SerializeField] private MinMax scaleMultiplier;
        [SerializeField] private MinMax baseZoneScale;
        [SerializeField] private MinMax mainZoneScale;
        
        public override bool OnInteract(StructureBase structureBase)
        {
            var data = structureBase.GetData<VisionStructureData>();
            var price = data.GetCurrentPrice();
            var hasMainZone = ZoneGeneratorManager.HasMainZone();
            if (!PlayerCollectibleManager.HasCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SOUL, price) && hasMainZone) return false;

            if (hasMainZone)
            {
                PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.SOUL, -price);
                data.MultiplyCurrentPrice(transactionMultiplier);
            }
            
            if (structureBase.WasUsed) ZoneGeneratorManager.ExpandZone(data.Key, scaleMultiplier.RandomFloat());
            else ZoneGeneratorManager.GenerateZone(structureBase.transform.position, data.Key, hasMainZone ? baseZoneScale.RandomFloat() : mainZoneScale.RandomFloat());

            return true;
        }

        public void SetZone(StructureBase structureBase)
        {
            var data = structureBase.GetData<VisionStructureData>();
            data.InitPrice(baseCost + GameManager.StageCount * stageBaseMultiplier, structureBase.GetInstanceID());
            structureBase.HandleInteraction();
        }

        public override string GetInteractionMessage(StructureBase structureBase)
        {
            var data = structureBase.GetData<VisionStructureData>();
            if (!data.IsInitialized()) data.Init().InitPrice(baseCost + GameManager.StageCount * stageBaseMultiplier, structureBase.GetInstanceID());
            return base.GetInteractionMessage(structureBase).Replace("$x$", data.GetCurrentPrice().ToString());
        }
        
        private class VisionStructureData : BaseStructureData<VisionStructureData>
        {
            private int _currentPrice;
            private string _key;

            public string Key => _key;
            
            public int GetCurrentPrice() => _currentPrice;

            public void InitPrice(int price, int instanceID)
            {
                _currentPrice = price;
                _key = $"VISION_{instanceID}";
            }

            public void MultiplyCurrentPrice(int multiplier)
            {
                _currentPrice *= multiplier;
            }
        }
    }
}