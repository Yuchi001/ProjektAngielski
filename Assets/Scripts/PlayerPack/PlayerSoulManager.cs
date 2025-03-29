using UnityEngine;

namespace PlayerPack
{
    public class PlayerSoulManager : MonoBehaviour
    {
        public delegate void SoulCountChangeDelegate(int amount, int current);
        public static event SoulCountChangeDelegate OnSoulCountChange;
        
        private int _currentCount = 99999;

        public int GetCurrentSoulCount() => _currentCount;

        public void AddSouls(int count)
        {
            _currentCount += count;
            OnSoulCountChange?.Invoke(count, _currentCount);
        }
    }
}