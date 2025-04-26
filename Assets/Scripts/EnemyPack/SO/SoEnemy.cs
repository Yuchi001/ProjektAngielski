using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack.Enums;
using EnemyPack.States.StateData;
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
        [SerializeField] private int damage;
        [SerializeField, Range(1, 10)] private int difficulty = 1;
        [SerializeField] private List<ERegionType> occurenceList;
        [SerializeField] private ESpriteRotation spriteRotation = ESpriteRotation.RotateLeft;
        [SerializeField] private bool useCustomDropCount;
        [SerializeField] private MinMax soulDropCount;
        [SerializeField] private MinMax scrapDropCount;
        [SerializeField] private List<StateDataBase> statesData;
        
        [Header("Behaviour")]
        [SerializeField] private EEnemyBehaviour enemyBehaviour;

        public Sprite EnemySprite => sprite;
        public EEnemyBehaviour EnemyBehaviour => enemyBehaviour;
        public List<ERegionType> OccurenceList => occurenceList;
        public T GetStateData<T>() where T: StateDataBase => statesData.First(s => s.Is<T>()).As<T>();
        public StateDataBase GetStateData(Type type) => statesData.FirstOrDefault(type.IsInstanceOfType);
        public void SetStatesData(List<StateDataBase> _statesData) => statesData = new List<StateDataBase>(_statesData);
        public float BodyScale => bodyScale;
        public ESpriteRotation SpriteRotation => spriteRotation;
        public AnimationClip WalkingAnimationClip => walkingAnimationClip;
        public int MaxHealth => maxHealth;
        public float AnimationSpeed => animationSpeed;
        public int Difficulty => difficulty;

        public int GetSoulDropCount()
        {
            if (useCustomDropCount) return soulDropCount.RandomInt();
            var modDiff = difficulty / 3;
            return new MinMax(Mathf.Max(1, modDiff), modDiff + 1).RandomInt();
        }

        public int GetScrapDropCount()
        {
            return scrapDropCount.RandomInt();
        }
    }
}