using UnityEngine;

namespace PlayerPack
{
    public class PlayerCoinManager : MonoBehaviour
    {
        public delegate void CoinCountChange(int amount, int current);
        public static event CoinCountChange OnCoinCountChange;

        private int _currentCount = 0;

        public int GetCurrentCount() => _currentCount;
        
        public void AddCoins(int count)
        {
            _currentCount += count;
            OnCoinCountChange?.Invoke(count, _currentCount);
        }
    }
}