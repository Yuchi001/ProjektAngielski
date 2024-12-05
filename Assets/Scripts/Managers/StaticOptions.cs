using System;
using ItemPack.Enums;
using Random = UnityEngine.Random;

namespace Managers
{
    public static class StaticOptions
    {
        public static readonly int MAX_TIER = 2;
        public static readonly int LEVEL_UP_PER_WEAPONS_COUNT = 3;

        public static readonly int MAX_CDR_PERCENTAGE = 80;
        
        // DEXTERITY
        public static readonly float DASH_STACK_PER_DEXTERITY_POINT = 0.1f; // TODO: balans
        public static readonly float MOVEMENT_SPEED_PER_DEXTERITY_POINT = 0.05f;
        
        // STRENGTH
        public static readonly float ADDITIONAL_CAPACITY_PER_STRENGTH_POINT = 0.1f; // TODO: balans
        public static readonly float MAX_HEALTH_PER_STRENGTH_POINT = 1; // TODO: balans
        
        // INTELIGENCE
        public static readonly float CDR_PER_INTELLIGENCE_POINT = 0.5f;

        public static float GetItemScaling(EScalingPower scalingPower)
        {
            return scalingPower switch
            {
                EScalingPower.NONE => 0,
                EScalingPower.BAD => Random.Range(25, 51) / 100f,
                EScalingPower.GOOD => Random.Range(50, 76) / 100f,
                EScalingPower.PERFECT => Random.Range(75, 101) / 100f,
                _ => 0
            };
        }
    }
}