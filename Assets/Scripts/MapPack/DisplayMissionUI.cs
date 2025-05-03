using TMPro;
using UIPack;
using UnityEngine;

namespace MapPack
{
    public class DisplayMissionUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI difficultyField;
        [SerializeField] private TextMeshProUGUI regionField;
        [SerializeField] private TextMeshProUGUI rewardField;
        [SerializeField] private TextMeshProUGUI soulCountField;
        
        public void Setup(MapManager.MissionData missionData)
        {
            difficultyField.text = $"Difficulty: {missionData.Difficulty}";
            regionField.text = $"Region: {missionData.RegionType}";
            rewardField.text = $"Rewards: {missionData.CoinReward}$";
            soulCountField.text = $"Souls: {missionData.SoulCount}";
        }
    }
}