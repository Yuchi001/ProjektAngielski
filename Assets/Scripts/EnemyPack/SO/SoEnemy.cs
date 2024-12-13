using System.Collections.Generic;
using EnemyPack.Enums;
using EnemyPack.States;
using ExpPackage.Enums;
using MapGeneratorPack.Enums;
using Other;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyPack.SO
{
    [CreateAssetMenu(fileName = "new Enemy", menuName = "Custom/Enemy")]
    public class  SoEnemy : SoPoolObject
    {
        [SerializeField] private AnimationClip walkingAnimationClip;
        [SerializeField] private int maxHealth;
        [SerializeField] private float bodyScale = 1;
        [SerializeField] private float movementSpeed;
        [SerializeField] private int damage;
        [SerializeField] private EExpGemType expGemType;
        [SerializeField] private EShootType shootType;
        [SerializeField] private List<EStageType> occurenceList;
        
        [Header("Behaviour")]
        [SerializeField] private EEnemyBehaviour enemyBehaviour;

        public EEnemyBehaviour EnemyBehaviour => enemyBehaviour;
        public List<EStageType> OccurenceList => occurenceList;
        public float BodyScale => bodyScale;
        public int Damage => damage;
        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public EExpGemType ExpGemType => expGemType;
        public EShootType ShootType => shootType;
    }
}