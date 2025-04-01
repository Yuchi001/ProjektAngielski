using Managers;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission Board Structure", menuName = "Custom/Structure/MissionBoard")]
    public class SoMissionBoardStructure : SoStructure
    {
        private string KEY => uIPrefabName;
        
        public override IOpenStrategy GetOpenStrategy(StructureBase structureBase)
        {
            var prefab = GameManager.GetPrefab<MissionBoardUI>(KEY);
            prefab.Setup(this, structureBase);
            return new SingletonOpenStrategy<MissionBoardUI>(prefab);
        }

        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            if (!structureBase.Toggle) UIManager.OpenUI<MissionBoardUI>(KEY, openStrategy, closeStrategy);
            else UIManager.CloseUI(KEY);
            return true;
        }
    }
}