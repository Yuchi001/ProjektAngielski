using System.Collections.Generic;
using System.Linq;
using PlayerPack.SO;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerStats : MonoBehaviour
    {
        private List<PlayerStat> _playerStats = new();
        
        public void Setup(SoCharacter pickedCharacter)
        {
            _playerStats = new List<PlayerStat>(pickedCharacter.PlayerStats);
        }

        public float GetStat(EPlayerStat type)
        {
            return _playerStats.FirstOrDefault(s => s.type == type)?.value ?? 1;
        }

        public void SetStat(EPlayerStat type, float value)
        {
            var stat = _playerStats.FirstOrDefault(s => s.type == type);
            if (stat == default) return;

            stat.value = value;
        }
        
        public void ModifyStat(EPlayerStat type, float amount)
        {
            var stat = _playerStats.FirstOrDefault(s => s.type == type);
            if (stat == default) return;

            stat.value += amount;
        }
    }
}