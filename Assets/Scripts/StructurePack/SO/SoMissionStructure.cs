using Managers;
using Managers.Other;
using MapPack;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Mission", menuName = "Custom/Structure/Mission")]
    public class SoMissionStructure : SoStructure
    {
        public override void OnFocusChanged(StructureBase structureBase, bool newValue)
        {
            structureBase.GetData<MissionStructureData>().ToggleUI(structureBase, newValue);
        }

        public override bool OnInteract(StructureBase structureBase)
        {
            structureBase.GetData<MissionStructureData>().StartRun();
            return true;
        }

        public static object CreateMissionStructureData(MapManager.MissionData missionData)
        {
            var data = new MissionStructureData();
            data.Init().SetMission(missionData);
            return data;
        }

        private class MissionStructureData : BaseStructureData<MissionStructureData>
        {
            private DefaultCloseStrategy _closeStrategy;
            private SingletonOpenStrategy<DisplayMissionUI> _openStrategy;

            private const string UI_KEY = "MISSION_DISPLAY_UI_KEY";

            private MapManager.MissionData _missionData;
            
            public override MissionStructureData Init()
            {
                var prefab = GameManager.GetPrefab<DisplayMissionUI>(PrefabNames.DisplayMissionUI);
                _openStrategy = new SingletonOpenStrategy<DisplayMissionUI>(prefab);
                _closeStrategy = new DefaultCloseStrategy();
                return base.Init();
            }

            public void SetMission(MapManager.MissionData missionData)
            {
                _missionData = missionData;
            }

            public void ToggleUI(StructureBase structureBase, bool open)
            {
                if (!open)
                {
                    UIManager.CloseUI(UI_KEY);
                    return;
                }
                
                UIManager.OpenUI<DisplayMissionUI>(UI_KEY, _openStrategy, _closeStrategy).Setup(_missionData);
                UIManager.ChangeToWorldPos(UI_KEY, structureBase.transform.position + new Vector3(0, 0.3f));
            }

            public void StartRun()
            {
                GameManager.StartRun(_missionData);
            }
        }
    }
}