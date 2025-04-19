using System.Collections.Generic;
using System.Linq;
using ItemPack.SO;
using PlayerPack.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerPack.SO
{
    [CreateAssetMenu(fileName = "new Character", menuName = "Custom/Character")]
    public class SoCharacter : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Sprite characterIcon;
        [SerializeField] private AnimationClip walkingAnim;
        [SerializeField] private AnimationClip idleAnim;
        [SerializeField] private Color characterColor;
        [SerializeField] private List<PlayerStatPair> stats;
        
        [SerializeField] private SoInventoryItem startingItem;

        public Color CharacterColor => characterColor;
        public Dictionary<EPlayerStatType, float> StatDict => stats.ToDictionary(s => s.StatType, s => s.StatValue);
        public string CharacterName => characterName;
        public string ID => id;
        public Sprite CharacterSprite => characterSprite;
        public Sprite CharacterIcon => characterIcon;
        public AnimationClip WalkingAnimation => walkingAnim;
        public AnimationClip IdleAnimation => idleAnim;
        public SoInventoryItem StartingItem => startingItem;

        public void SetStats(List<PlayerStatPair> stats)
        {
            this.stats = stats;
        }

        [System.Serializable]
        public class PlayerStatPair
        {
            [SerializeField] private EPlayerStatType statType;
            [SerializeField] private float statValue;

            public EPlayerStatType StatType => statType;
            public float StatValue => statValue;

            public PlayerStatPair(EPlayerStatType statType, float statValue)
            {
                this.statType = statType;
                this.statValue = statValue;
            }
        }
    }
}