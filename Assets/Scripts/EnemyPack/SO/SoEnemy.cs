using EnemyPack.Enums;
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
        [SerializeField] private bool playerSpeed;
        [SerializeField] private EExpGemType expGemType;
        [SerializeField] private bool isHorde;
        [SerializeField] private bool isHeavy;
        [SerializeField] private EEnemyState enemyState;

        [FormerlySerializedAs("enemyActionCooldown")] [SerializeField] private float actionCooldown;
        [SerializeField] private GameObject enemyAdditionalLogic = null;

        public float BodyScale => bodyScale;
        public bool IsHorde => isHorde;
        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public bool PlayerSpeed => playerSpeed;
        public EExpGemType ExpGemType => expGemType;
        public float ActionCooldown => actionCooldown;
        public bool IsHeavy => isHeavy;
        public EEnemyState EnemyState => enemyState;
        public void SpawnEnemyLogic(EnemyLogic enemyLogic)
        {
            if (enemyAdditionalLogic == null) return;

            var enemyActionScript =
                Instantiate(enemyAdditionalLogic, enemyLogic.transform.position, Quaternion.identity,
                    enemyLogic.transform).GetComponent<EnemyAction>();
            enemyActionScript.Setup(enemyLogic, this);
        }
    }
}