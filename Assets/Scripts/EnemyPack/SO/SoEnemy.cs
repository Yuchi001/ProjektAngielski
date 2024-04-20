using ExpPackage.Enums;
using UnityEngine;

namespace EnemyPack.SO
{
    [CreateAssetMenu(fileName = "new Enemy", menuName = "Custom/Enemy")]
    public class SoEnemy : ScriptableObject
    {
        [SerializeField] private AnimationClip walkingAnimationClip;
        [SerializeField] private int maxHealth;
        [SerializeField] private float movementSpeed;
        [SerializeField] private EExpGemType expGemType;

        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public EExpGemType ExpGemType => expGemType;
    }
}