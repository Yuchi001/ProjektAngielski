using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack.Enums;
using EnemyPack.States.StateData;
using Other;
using UnityEngine;
using Utils;
using WorldGenerationPack.Enums;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        
#if UNITY_EDITOR
        public StateDataBase GetStateData(Type type)
        {
            var data = statesData.FirstOrDefault(data => data.GetType() == type);
            if (data != null) return data;

            data = (StateDataBase)CreateInstance(type);
            AssetDatabase.AddObjectToAsset(data, this); 
            statesData.Add(data);

            return data;
        }

        public void SetStatesData(List<StateDataBase> _statesData)
        {
            statesData = _statesData;
            var assetPath = AssetDatabase.GetAssetPath(this);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            foreach (var asset in assets)
            {
                if (asset == null || statesData.Contains(asset)) continue;
                
                DestroyImmediate(asset, true);
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public bool HasStateData() => statesData != null;
#endif
    }
}