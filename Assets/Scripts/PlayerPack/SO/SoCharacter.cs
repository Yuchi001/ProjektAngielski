using UnityEngine;
using WeaponPack.SO;

namespace PlayerPack.SO
{
    [CreateAssetMenu(fileName = "new Character", menuName = "Custom/Character")]
    public class SoCharacter : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private int maxHp;
        [SerializeField] private int maxWeaponsInEq;
        [SerializeField] private SoWeapon startingWeapon;
        [SerializeField] private float movementSpeed;
        [SerializeField] private AnimationClip walkingAnim;
        [SerializeField] private AnimationClip idleAnim;

        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public int MaxHp => maxHp;
        public int MaxWeaponsInEq => maxWeaponsInEq;
        public float MovementSpeed => movementSpeed;
        public AnimationClip WalkingAnimation => walkingAnim;
        public AnimationClip IdleAnimation => idleAnim;
        public SoWeapon StartingWeapon => startingWeapon;
    }
}