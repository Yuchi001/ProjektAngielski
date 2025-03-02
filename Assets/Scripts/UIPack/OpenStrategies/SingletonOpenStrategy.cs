using System;
using System.Linq;

namespace UIPack.OpenStrategies
{
    public class SingletonOpenStrategy<T> : IOpenStrategy
    {
        private readonly Type _instanceType = typeof(T);
        private readonly UIBase _basePrefab;

        public SingletonOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }

        public bool Open(out UIBase uiBase, string key)
        {
            var record = UIManager.GetCurrentUIBaseList().FirstOrDefault(ui => ui.GetType() == _instanceType);
            uiBase = record == default ? UIManager.SpawnUI(_basePrefab) : record.Script;
            uiBase.OnOpen(key);

            return true;
        }
    }
}