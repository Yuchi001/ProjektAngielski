using Object = UnityEngine.Object;

namespace UIPack.CloseStrategies
{
    public class DestroyCloseStrategy : ICloseStrategy
    {
        private readonly float _destroyTime;
        private readonly string _key;

        public DestroyCloseStrategy(string key, float destroyTime = 0.3f)
        {
            _key = key;
            _destroyTime = destroyTime;
        }
        
        public void Close(UIBase spawnedBase)
        {
            UIManager.RemoveUI(_key);
            spawnedBase.OnClose();
            Object.Destroy(spawnedBase.gameObject, _destroyTime);
        }
    }
}