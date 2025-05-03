using System;
using UnityEngine;
using Utils;

namespace MapPack
{
    public static class DifficultyExtensions
    {
        public static MinMax EnemyDifficultyBounds(this EDifficulty difficulty)
        {
            const int MIN_EXPERT_DIFF_BOUND = 4;
            const int MIN_DIFF = 1;
            const int MAX_DIFF = 10;
            var min = difficulty == EDifficulty.EXPERT ? MIN_EXPERT_DIFF_BOUND : 1;
            var max = Mathf.Clamp((int)difficulty * 3 + 4, MIN_DIFF, MAX_DIFF);
            return new MinMax(min, max);
        }

        public static float MissionLengthEstimation(this EDifficulty difficulty)
        {
            const float BASE_ESTIMATION = 240;
            const float ESTIMATION_PER_DIFF = 60;
            return BASE_ESTIMATION + ESTIMATION_PER_DIFF * (int)difficulty;
        }

        public static string GetTranslation(this EDifficulty difficulty) => difficulty switch
        {
            EDifficulty.EASY => "Easy",
            EDifficulty.MEDIUM => "Medium",
            EDifficulty.HARD => "Hard",
            EDifficulty.EXPERT => "Expert",
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
        
        public enum EDifficulty
        {
            EASY = 0,
            MEDIUM = 1,
            HARD = 2,
            EXPERT = 3
        }
    }
}