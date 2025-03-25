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
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private AnimationClip walkingAnim;
        [SerializeField] private AnimationClip idleAnim;
        [SerializeField] private Color characterColor;
        [SerializeField] private List<PlayerStatPair> stats;
        
        [FormerlySerializedAs("startingItem")] [SerializeField] private SoInventoryItem startingInventoryItem;

        public Color CharacterColor => characterColor;
        public Dictionary<EPlayerStatType, float> StatDict => stats.ToDictionary(s => s.StatType, s => s.StatValue);
        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public AnimationClip WalkingAnimation => walkingAnim;
        public AnimationClip IdleAnimation => idleAnim;
        public SoInventoryItem StartingInventoryItem => startingInventoryItem;

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