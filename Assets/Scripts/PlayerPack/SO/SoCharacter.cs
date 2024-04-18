using UnityEngine;

namespace PlayerPack.SO
{
    [CreateAssetMenu(fileName = "new Character", menuName = "Custom/Character")]
    public class SoCharacter : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private int maxHp;
        
        // todo: Implement weapon system
        
        [SerializeField] private float movementSpeed;
        [SerializeField] private AnimationClip walkingAnim;

        public string CharacterName => characterName;
        public Sprite CharacterSprite => characterSprite;
        public int MaxHp => maxHp;
        public float MovementSpeed => movementSpeed;
        public AnimationClip WalkingAnimation => walkingAnim;
    }
}