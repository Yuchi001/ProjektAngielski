using System.Collections.Generic;
using EnemyPack.Enums;
using Other;
using UnityEngine;
using Utils;
using WorldGenerationPack.Enums;

namespace EnemyPack.SO
{
    [CreateAssetMenu(fileName = "new Enemy", menuName = "Custom/Enemy")]
    public class  SoEnemy : SoPoolObject
    {
        [Header("General")] 
        [SerializeField] private Sprite sprite;
        [SerializeField] private AnimationClip walkingAnimationClip;
        [SerializeField] private int maxHealth;
        [SerializeField] private float bodyScale = 1;
        [SerializeField] private float animationSpeed = 1;
        [SerializeField] private float movementSpeed;
        [SerializeField, Min(0.1f)] private float attackSpeed;
        [SerializeField] private int damage;
        [SerializeField, Range(1, 10)] private int difficulty = 1;
        [SerializeField] private List<ERegionType> occurenceList;
        [SerializeField] private ESpriteRotation spriteRotation = ESpriteRotation.RotateLeft;
        [SerializeField] private bool useCustomDropCount;
        [SerializeField] private MinMax soulDropCount;
        [SerializeField] private MinMax scrapDropCount;
        
        [Header("Behaviour")]
        [SerializeField] private EEnemyBehaviour enemyBehaviour;

        public Sprite EnemySprite => sprite;
        public EEnemyBehaviour EnemyBehaviour => enemyBehaviour;
        public List<ERegionType> OccurenceList => occurenceList;
        public float BodyScale => bodyScale;
        public ESpriteRotation SpriteRotation => spriteRotation;
        public int Damage => damage;
        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public float AnimationSpeed => animationSpeed;
        public float AttackSpeed => attackSpeed;
        public int Difficulty => difficulty;

        public int GetSoulDropCount()
        {
            if (useCustomDropCount) return soulDropCount.RandomInt();
            var halfDiff = difficulty / 2;
            return new MinMax(Mathf.Max(1, halfDiff), halfDiff + 2).RandomInt();
        }

        public int GetScrapDropCount()
        {
            return scrapDropCount.RandomInt();
        }
    }
}