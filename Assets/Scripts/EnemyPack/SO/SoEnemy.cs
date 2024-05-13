using ExpPackage.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyPack.SO
{
    [CreateAssetMenu(fileName = "new Enemy", menuName = "Custom/Enemy")]
    public class SoEnemy : ScriptableObject
    {
        [SerializeField] private AnimationClip walkingAnimationClip;
        [SerializeField] private int maxHealth;
        [SerializeField] private float bodyScale = 1;
        [SerializeField] private float movementSpeed;
        [SerializeField] private EExpGemType expGemType;
        [SerializeField] private bool isHorde;

        [SerializeField] private bool useAction = false;
        [SerializeField] private bool oneTimeSpawnAction = false;
        [FormerlySerializedAs("enemyActionCooldown")] [SerializeField] private float actionCooldown;
        [SerializeField] private GameObject enemyAction = null;

        public float BodyScale => bodyScale;
        public bool IsHorde => isHorde;
        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public EExpGemType ExpGemType => expGemType;
        public bool UseAction => useAction;
        public bool OnTimeSpawnLogic => oneTimeSpawnAction;
        public float ActionCooldown => actionCooldown;
        public GameObject EnemyAction => enemyAction;
    }
}