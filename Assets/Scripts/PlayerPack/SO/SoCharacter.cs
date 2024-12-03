using ItemPack.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerPack.SO
{
    [CreateAssetMenu(fileName = "new Character", menuName = "Custom/Character")]
    public class SoCharacter : ScriptableObject
    {
        [Header("Other")]
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private AnimationClip walkingAnim;
        [SerializeField] private AnimationClip idleAnim;
        [SerializeField] private Color characterColor;
        
        [Header("Stats")]
        [SerializeField] private int maxHp;
        [SerializeField] private int armor;
        [SerializeField] private int strength;
        [SerializeField] private int intelligence;
        [SerializeField] private int dexterity;
        [SerializeField] private int movementSpeed;
        [SerializeField] private SoItem startingItem;

        public Color CharacterColor => characterColor;
        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public int MaxHp => maxHp;
        public int Armor => armor;
        public int Strength => strength;
        public int Intelligence => intelligence;
        public int Dexterity => dexterity;
        public int MovementSpeed => movementSpeed;
        public AnimationClip WalkingAnimation => walkingAnim;
        public AnimationClip IdleAnimation => idleAnim;
        public SoItem StartingItem => startingItem;
    }
}