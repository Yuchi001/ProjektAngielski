using Managers;
using MapGeneratorPack;
using PlayerPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Vision Structure", menuName = "Custom/Structure/Vision")]
    public class SoVisionStructure : SoStructure
    {
        [SerializeField] private int baseCost;
        [SerializeField] private int stageBaseMultiplier;
        [SerializeField] private int transactionMultiplier;
        [SerializeField] private float scaleMultiplier = 0.2f;
        [SerializeField] private float baseZoneScale = 2;
        
        private static PlayerSoulManager PlayerSoulManager => PlayerManager.PlayerSoulManager;
        
        public override bool OnInteract(StructureBase structureBase)
        {
            var data = structureBase.GetData<VisionStructureData>();
            var price = data.GetCurrentPrice();
            if (PlayerSoulManager.GetCurrentSoulCount() < price) return false;

            PlayerSoulManager.AddSouls(-price);
            data.MultiplyCurrentPrice(transactionMultiplier);
            
            if (structureBase.WasUsed) MapGeneratorManager.ExpandZone(data.GetKey(), scaleMultiplier);
            else MapGeneratorManager.GenerateZone(structureBase.transform.position, data.GetKey(), baseZoneScale);

            return true;
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
            private int _instanceID;

            public string GetKey() => $"VISION_{_instanceID}";
            public int GetCurrentPrice() => _currentPrice;

            public void InitPrice(int price, int instanceID)
            {
                _currentPrice = price;
                _instanceID = instanceID;
            }

            public void MultiplyCurrentPrice(int multiplier)
            {
                _currentPrice *= multiplier;
            }
        }
    }
}