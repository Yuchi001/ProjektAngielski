using System;
using System.Linq;

namespace UIPack.OpenStrategies
{
    public class AlwaysNewOpenStrategy<T> : IOpenStrategy where T: UIBase
    {
        private readonly Type _instanceType = typeof(T);
        private readonly UIBase _basePrefab;

        public AlwaysNewOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }

        public bool Open(out UIBase uiBase, string key)
        {
            UIManager.CloseUI(key, true);
            uiBase = UIManager.SpawnUI(_basePrefab);
            
            uiBase.OnOpen(key);

            return true;
        }
    }
}