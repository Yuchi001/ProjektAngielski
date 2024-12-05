using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerPack.Enums
{
    public enum EPlayerStatType
    {
        Strength,
        Intelligence,
        Dexterity,
        DashStacks,
        MaxHealth,
        Capacity,
        MovementSpeed,
        CooldownReduction,
        Health,
    }

    public static class PlayerStatUtils
    {
        public static string DependenciesToStr(IEnumerable<EPlayerStatType> stats)
        {
            return string.Join("|", stats.Select(DependencyToStr));
        }

        public static string DependencyToStr(EPlayerStatType stat)
        {
            return stat switch
            {
                EPlayerStatType.Strength => "STR",
                EPlayerStatType.Intelligence => "INT",
                EPlayerStatType.Dexterity => "DEX",
                EPlayerStatType.DashStacks => "DS",
                EPlayerStatType.MaxHealth => "MAX_HP",
                EPlayerStatType.Capacity => "CAP",
                EPlayerStatType.MovementSpeed => "MS",
                EPlayerStatType.CooldownReduction => "CDR",
                EPlayerStatType.Health => "HP",
                _ => "UNDEFINED"
            };
        }
    }
}