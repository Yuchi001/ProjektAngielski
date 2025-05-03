using System;
using System.Linq;

namespace UIPack.OpenStrategies
{
    public class SingletonOpenStrategy<T> : IOpenStrategy where T: UIBase
    {
        private readonly Type _instanceType = typeof(T);
        private readonly UIBase _basePrefab;

        public SingletonOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }

        public bool Open(out UIBase uiBase, string key)
        {
            var record = UIManager.GetCurrentUIBaseList().FirstOrDefault(ui => _instanceType.IsInstanceOfType(ui.Script));
            uiBase = record == default ? UIManager.SpawnUI(_basePrefab) : record.Script;
            uiBase.OnOpen(key);

            return true;
        }
    }
}