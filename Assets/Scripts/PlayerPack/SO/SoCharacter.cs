using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using WeaponPack.SO;

namespace PlayerPack.SO
{
    [CreateAssetMenu(fileName = "new Character", menuName = "Custom/Character")]
    public class SoCharacter : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private SoWeapon startingWeapon;
        [SerializeField] private AnimationClip walkingAnim;
        [SerializeField] private AnimationClip idleAnim;
        [SerializeField] private Color characterColor;
        [SerializeField] private List<PlayerStat> playerStats;

        public Color CharacterColor => characterColor;
        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public List<PlayerStat> PlayerStats => playerStats;
        public AnimationClip WalkingAnimation => walkingAnim;
        public AnimationClip IdleAnimation => idleAnim;
        public SoWeapon StartingWeapon => startingWeapon;
    }

    [System.Serializable]
    public class PlayerStat
    {
        public float value;
        public EPlayerStat type;
    }
}