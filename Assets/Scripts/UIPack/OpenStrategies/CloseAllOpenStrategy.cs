namespace UIPack.OpenStrategies
{
    public class CloseAllOpenStrategy : IOpenStrategy
    {
        private readonly UIBase _basePrefab;
        
        public bool Open(out UIBase uiBase, string key)
        {
            UIManager.CloseAllUIs();
            uiBase = UIManager.SpawnUI(_basePrefab);
            uiBase.OnOpen(key);

            return true;
        }
    }
}