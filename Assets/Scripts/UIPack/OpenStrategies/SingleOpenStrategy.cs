namespace UIPack.OpenStrategies
{
    public class SingleOpenStrategy : IOpenStrategy
    {
        private readonly UIBase _basePrefab;
        
        public SingleOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }
        
        public bool Open(out UIBase uiBase)
        {
            uiBase = UIManager.SpawnUI(_basePrefab);
            uiBase.Animator.SetTrigger("OPEN");
            uiBase.OnOpen();
            return true;
        }
    }
}