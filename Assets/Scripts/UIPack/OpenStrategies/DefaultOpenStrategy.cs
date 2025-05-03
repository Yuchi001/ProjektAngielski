namespace UIPack.OpenStrategies
{
    public class DefaultOpenStrategy : IOpenStrategy
    {
        private readonly UIBase _basePrefab;
        
        public DefaultOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }
        
        public bool Open(out UIBase uiBase, string key)
        {
            uiBase = UIManager.SpawnUI(_basePrefab);
            uiBase.OnOpen(key);
            return true;
        }
    }
}