using System;
using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using Managers.Enums;
using MapPack;
using StructurePack.SO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using WorldGenerationPack.Enums;
using Random = UnityEngine.Random;

namespace StagePack
{
    [CreateAssetMenu(fileName = "new Region Data", menuName = "Custom/Region")]
    public class SoRegion : ScriptableObject
    {
        [SerializeField] private ERegionType regionType;
        [SerializeField] private EThemeType regionTheme;
        [SerializeField] private List<StructureGenerationData> structures;
        [SerializeField] private List<SoStageEffect> uniqueEffects;
        [SerializeField] private EnemySpawner uniqueSpawner;
        [SerializeField] private MinMax soulCountPerDiff;
        [SerializeField] private MinMax soulCountBase;
        [SerializeField] private MinMax coinRewardPerDiff;
        [SerializeField] private MinMax coinRewardBase;
        [SerializeField] private AnimationCurve difficultyScalingCurve;
        [SerializeField] private List<TileBase> tileVariants;
        [SerializeField] private TileBase bottomTile;
        [SerializeField] private TileBase topTile;
        [SerializeField] private TileBase leftTile;
        [SerializeField] private TileBase rightTile;
        [SerializeField] private TileBase topRightCornerTile;
        [SerializeField] private TileBase topLeftCornerTile;
        [SerializeField] private TileBase bottomRightCornerTile;
        [SerializeField] private TileBase bottomLeftCornerTile;
        [SerializeField] private MinMax averageWorldSizeX;
        [SerializeField] private MinMax averageWorldSizeY;
        [SerializeField] private Sprite backgroundSprite;

        public ERegionType RegionType => regionType;
        public EThemeType RegionTheme => regionTheme;

        public TileBase GetTile(int x, int y, Vector2 worldSize) => GetOrientation(x, y, worldSize) switch
        {
            EOrientation.TOP => topTile,
            EOrientation.BOTTOM => bottomTile,
            EOrientation.RIGHT => rightTile,
            EOrientation.LEFT => leftTile,
            EOrientation.TOP_RIGHT => topRightCornerTile,
            EOrientation.TOP_LEFT => topLeftCornerTile,
            EOrientation.BOTTOM_RIGHT => bottomRightCornerTile,
            EOrientation.BOTTOM_LEFT => bottomLeftCornerTile,
            null => tileVariants.RandomElement(),
            _ => null
        };

        public EnemySpawner EnemySpawner => uniqueSpawner;

        private static EOrientation? GetOrientation(int x, int y, Vector2 worldSize)
        {
            var maxX = (int)worldSize.x - 1;
            var maxY = (int)worldSize.y - 1;

            var isLeft = x == 0;
            var isRight = x == maxX;
            var isTop = y == maxY;
            var isBottom = y == 0;

            switch (isTop)
            {
                case true when isLeft:
                    return EOrientation.TOP_LEFT;
                case true when isRight:
                    return EOrientation.TOP_RIGHT;
            }

            switch (isBottom)
            {
                case true when isLeft:
                    return EOrientation.BOTTOM_LEFT;
                case true when isRight:
                    return EOrientation.BOTTOM_RIGHT;
            }

            if (isTop) return EOrientation.TOP;
            if (isBottom) return EOrientation.BOTTOM;
            if (isLeft) return EOrientation.LEFT;
            if (isRight) return EOrientation.RIGHT;

            return null;
        }

        public Sprite BackgroundSprite => backgroundSprite;

        public List<(Vector2Int position, SoStructure structure)> GetStructures(Vector2Int worldSize)
        {
            var list = new List<(Vector2Int position, SoStructure structure)>();
            var positions = new List<Vector2Int>();
            foreach (var pair in structures)
            {
                foreach (var structure in pair.GetStructures(worldSize))
                {
                    var tries = 0;
                    const int maxTries = 10;
                    Vector2Int position;
                    do
                    {
                        var x = Random.Range(1, worldSize.x - 1);
                        var y = Random.Range(1, worldSize.y - 1);
                        position = new Vector2Int(x, y);
                        tries++;
                    } while (!isValid(position, pair) || tries > maxTries);
                    if (tries + 1 == maxTries) continue;
                    positions.Add(position);
                    list.Add((position, structure));
                }
            }

            return list;

            bool isValid(Vector2Int position, StructureGenerationData data)
            {
                return !positions.Contains(position);
            }
        }
        
        public IEnumerable<SoStageEffect> UniqueEffects => uniqueEffects;
        public int RandomSoulCount(DifficultyExtensions.EDifficulty difficulty)
        {
            var result = soulCountBase.RandomInt();
            var diff = (int)difficulty;
            for (var i = 0; i < diff; i++) result += soulCountPerDiff.RandomInt();

            return result;
        }

        public int RandomReward(DifficultyExtensions.EDifficulty difficulty)
        {
            var result = coinRewardBase.RandomInt();
            var diff = (int)difficulty;
            for (var i = 0; i < diff; i++)
            {
                result += coinRewardPerDiff.RandomInt();
            }

            return result;
        }
        
        public float GetScaledDifficulty(float time, DifficultyExtensions.EDifficulty difficulty)
        {
            return difficultyScalingCurve.Evaluate(time / difficulty.MissionLengthEstimation());
        }

        public Vector2Int GetWorldSize() => new(averageWorldSizeX.RandomInt(), averageWorldSizeY.RandomInt());

        private enum EOrientation
        {
            TOP,
            BOTTOM,
            RIGHT,
            LEFT,
            TOP_RIGHT,
            TOP_LEFT,
            BOTTOM_RIGHT,
            BOTTOM_LEFT,
        }

        [System.Serializable]
        public struct StructureGenerationData
        {
            [SerializeField] private SoStructure structure;
            [SerializeField] private float squaresPerStructure;
            
            public List<SoStructure> GetStructures(Vector2Int worldSize)
            {
                var list = new List<SoStructure>();
                var count = worldSize.x * worldSize.y / squaresPerStructure;
                for (var i = 0; i < count; i++) list.Add(structure);

                return list;
            }
        }
    }
}